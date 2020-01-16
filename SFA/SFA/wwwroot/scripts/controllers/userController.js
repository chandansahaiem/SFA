app.controller('userController', function ($scope, $window, $location, $mdDialog, userService, roleService) {
    var id = $location.absUrl().substr($location.absUrl().lastIndexOf('detail/') + 7);
    $scope.user = {};
    $scope.isDisabled = false;

    $scope.backToMain = function () {
        $window.location.href = '/home';
    };
    $scope.backToList = function () {
        $window.location.href = '/user';
    };

    $scope.save = function () {
        $scope.isDisabled = true;

        userService.save($scope.user).then(function (resp) {
            if (resp.data !== null && resp.data !== undefined && resp.data === true) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('ENFINS Admin')
                        .textContent('User Saved successfully')
                        .ariaLabel('Alert Dialog')
                        .ok('OK')
                ).then(function () {
                    $window.location.href = '/user';
                    $scope.isDisabled = false;
                });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('ENFINS Admin')
                        .textContent('Failed To Saved User')
                        .ok('OK')
                ).then(function () {
                    $scope.isDisabled = false;
                });
            }
        });
    };
    function init() {
        roleService.getAll().then(function (resp) {
            $scope.roles = resp.data;
        });
        if (id !== null && id !== undefined && id !== '00000000-0000-0000-0000-000000000000') {
            userService.get(id).then(function (resp) {
                $scope.user = resp;
            });
        }
    }

    init();
});