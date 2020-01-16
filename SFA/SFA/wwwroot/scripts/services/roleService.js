app.service('roleService', function ($http) {
    this.getAll = function () {
        return $http.get('/role/api/all');
    };
    this.search = function (query) {
        return $http.post('/role/api/search', query);
    };
    this.get = function (id) {
        return $http.get('/role/api/get/' + id).then(function (resp) {
            return resp.data;
        });
    };
    this.save = function (role) {
        return $http.post('/role/api/save', role).then(function (resp) {
            return resp;
        });
    };
});