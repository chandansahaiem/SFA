app.controller('usersController', function ($scope, $window, $mdDialog, userService, roleService) {
    $scope.filter = {
        options: {
            debounce: 500
        }
    };
    $scope.query = {
        name: '',
        limit: 20,
        order: 'name',
        page: 1
    };
    $scope.selected = [];
    $scope.showFilter = false;
    $scope.data = [];

    $scope.filter = function () {
        $scope.showFilter = true;
    };
    $scope.close = function () {
        $scope.showFilter = false;

        $scope.query = {
            name: '',
            limit: 20,
            order: 'name',
            page: 1
        };
        $scope.search();
    };


    $scope.$watch('query.name', function (newValue, oldValue) {
        if (!oldValue) {
            bookmark = $scope.query.page;
        }
        if (newValue !== oldValue) {
            $scope.query.page = 1;
        }
        if (!newValue) {
            $scope.query.page = bookmark;
        }

        $scope.search();
    });

    $scope.search = function () {
        $scope.promise = userService.search($scope.query).then(success);
    };
   
    $scope.detail = function (id) {
        if (id === undefined) {
            id = '00000000-0000-0000-0000-000000000000';
        }
        $window.location.href = '/user/detail/' + id;
    };

    function success(resp) {
        $scope.users = resp.data.result;
        $scope.count = resp.data.count;
    }
    function init() {
        roleService.getAll().then(function (resp) {
            $scope.roles = resp.data;
        });
        $scope.search();
    }
    init();
});
