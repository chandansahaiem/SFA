﻿(function () {
    var injectParams = ['$q', '$rootScope', '$timeout', '$window', 'httpInterceptor'];

    var overlayDirective = function ($q, $rootScope, $timeout, $window, httpInterceptor) {

        var template = '<div id="overlay-container" class="overlayContainer">' +
                                        '<div id="overlay-background" class="overlayBackground"></div>' +
                                        '<div id="overlay-content" class="overlayContent" data-ng-transclude>' +
                                        '</div>' +
                                    '</div>',

            link = function (scope, element, attrs) {
                var overlayContainer = null,
                    timerPromise = null,
                    timerPromiseHide = null,
                    queue = [];

                $rootScope.$on("showProcess", function (event, args) {
                    var content = document.getElementById('overlay-content-text');
                    content.innerHTML = args;
                });

                init();

                function init() {
                    wireUpHttpInterceptor();
                    if ($window.jQuery) wirejQueryInterceptor();
                    overlayContainer = element[0].firstChild; //Get to template
                }

                //Hook into httpInterceptor factory request/response/responseError functions
                function wireUpHttpInterceptor() {

                    httpInterceptor.request = function (config) {
                        processRequest();
                        return config || $q.when(config);
                    };

                    httpInterceptor.response = function (response) {
                        processResponse();
                        return response || $q.when(response);
                    };

                    httpInterceptor.responseError = function (rejection) {
                        processResponse();
                        return $q.reject(rejection);
                    };
                }

                //Monitor jQuery Ajax calls in case it's used in an app
                function wirejQueryInterceptor() {
                    $(document).ajaxStart(function () {
                        processRequest();
                    });

                    $(document).ajaxComplete(function () {
                        processResponse();
                    });

                    $(document).ajaxError(function () {
                        processResponse();
                    });
                }

                function processRequest() {
                    queue.push({});
                    if (queue.length == 1) {
                        timerPromise = $timeout(function () {
                            if (queue.length) showOverlay();
                        }, scope.viewOverlayDelay ? scope.viewOverlayDelay : 500); //Delay showing for 500 millis to avoid flicker
                    }
                }

                function processResponse() {
                    queue.pop();
                    if (queue.length == 0) {
                        //Since we don't know if another XHR request will be made, pause before
                        //hiding the overlay. If another XHR request comes in then the overlay
                        //will stay visible which prevents a flicker
                        timerPromiseHide = $timeout(function () {
                            //Make sure queue is still 0 since a new XHR request may have come in
                            //while timer was running
                            if (queue.length == 0) {
                                hideOverlay();
                                if (timerPromiseHide) $timeout.cancel(timerPromiseHide);
                            }
                        }, scope.viewOverlayDelay ? scope.viewOverlayDelay : 500);
                    }
                }

                function showOverlay() {
                    var w = 0;
                    var h = 0;
                    if (!$window.innerWidth) {
                        if (!(document.documentElement.clientWidth == 0)) {
                            w = document.documentElement.clientWidth;
                            h = document.documentElement.clientHeight;
                        }
                        else {
                            w = document.body.clientWidth;
                            h = document.body.clientHeight;
                        }
                    }
                    else {
                        w = $window.innerWidth;
                        h = $window.innerHeight;
                    }
                    var content = document.getElementById('overlay-content');
                    var contentWidth = parseInt(getComputedStyle(content, 'width').replace('px', ''));
                    var contentHeight = parseInt(getComputedStyle(content, 'height').replace('px', ''));

                    content.style.top = h / 2 - contentHeight / 2 + 'px';
                    content.style.left = w / 2 - contentWidth / 2 + 'px';

                    overlayContainer.style.display = 'block';
                }

                function hideOverlay() {
                    if (timerPromise) $timeout.cancel(timerPromise);
                    overlayContainer.style.display = 'none';
                }

                var getComputedStyle = function () {
                    var func = null;
                    if (document.defaultView && document.defaultView.getComputedStyle) {
                        func = document.defaultView.getComputedStyle;
                    } else if (typeof (document.body.currentStyle) !== "undefined") {
                        func = function (element, anything) {
                            return element["currentStyle"];
                        };
                    }

                    return function (element, style) {
                        return func(element, null)[style];
                    };
                }();
            };

        return {
            restrict: 'EA',
            transclude: true,
            scope: {
                overlayDelay: "@"
            },
            template: template,
            link: link
        };
    };

    var viewDirectivesApp = angular.module('app.directives', []);

    //Empty factory to hook into $httpProvider.interceptors
    //Directive will hookup request, response, and responseError interceptors
    viewDirectivesApp.factory('httpInterceptor', function () {
        return {};
    });

    //Hook httpInterceptor factory into the $httpProvider interceptors so that we can monitor XHR calls
    viewDirectivesApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('httpInterceptor');
    }]);

    //Directive that uses the httpInterceptor factory above to monitor XHR calls
    //When a call is made it displays an overlay and a content area
    //No attempt has been made at this point to test on older browsers
    overlayDirective.$inject = injectParams;
    viewDirectivesApp.directive('overlay', overlayDirective);
}());