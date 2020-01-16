app.service('userService', function ($http) {
    this.getAll = function () {
        return $http.get('/user/api/all');
    };
    this.search = function (query) {
        return $http.post('/user/api/search', query);
    };
    this.get = function (id) {
        return $http.get('/user/api/get/' + id).then(function (resp) {
            return resp.data;
        });
    };
    this.save = function (user) {
        return $http.post('/user/api/save', user).then(function (resp) {
            return resp;
        });
    };
});