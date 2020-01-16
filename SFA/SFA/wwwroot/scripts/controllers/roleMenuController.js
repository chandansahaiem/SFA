app.controller('roleMenuController', function ($scope, $window, $mdDialog, roleMenuService, roleService) {
    $scope.roleMenu = {};
    $scope.backToList = function () {
        $window.location.href = '/nav/systemAdmin';
    };

    $scope.getMenu = function () {
        roleMenuService.getMenuByRole($scope.roleMenu.roleId).then(function (resp) {
            $scope.roleMenu.menus = resp;
        });
    };

    $scope.save = function () {
        roleMenuService.saveRoleMenu($scope.roleMenu).then(function (resp) {
            if (resp.data !== null && resp.data !== undefined && resp.data === true) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('ENFINS Admin')
                        .textContent('Role Wise Menu Saved successfully')
                        .ariaLabel('Alert Dialog')
                        .ok('OK')
                ).then(function () {
                    $window.location.href = '/roleMenu';
                });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('ENFINS Admin')
                        .textContent('!Failed to Save Role Wise Menu')
                        .ariaLabel('Alert Dialog')
                        .ok('OK')
                );
            }
        });
    };

    function init() {        
        roleService.getAll().then(function (resp) {
            $scope.roles = resp.data;
        });
    }
    init();
});