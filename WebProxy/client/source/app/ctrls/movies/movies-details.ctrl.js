(function() {
    'use strict';

    angular
        .module('hr')
        .controller('MovieDetailsCtrl', ['$http', '$stateParams', '$state', MovieDetailsCtrl]);

    function MovieDetailsCtrl($http, $stateParams, $state) {
        var self = this;

        self.movieId = $stateParams.movieId;
        
        $http({
            method: 'get',
            url: 'https://localhost:44353/api/Movie/' + self.movieId
        }).then(function(response) {
            console.log(response, 'res');
            self.movie = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });

        self.delete = function(movieId) {
            $http({
                method: 'DELETE',
                url: 'https://localhost:44353/api/Movie?Id=' + movieId
            })
            .then(function(response) {
                console.log('succes');
                $state.go('movies');
            }, function(error) {
                console.log('can not put data.');
            });
        };
    };
}());