(function() {
    'use strict';

    angular
        .module('hr')
        .controller('MovieEditCtrl', ['$http', '$stateParams', '$state', MovieEditCtrl]);

    function MovieEditCtrl($http, $stateParams, $state) {
        var self = this;

        console.log($stateParams.movieId)
        self.movieId = $stateParams.movieId

        $http({
            method: 'get',
            url: 'https://localhost:44353/api/Movie/' + self.movieId
        }).then(function(response) {
            console.log(response, 'res');
            self.movie = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });

        self.update = function(movie) {
            self.updatedMovie = {
                "id": movie.id,
                "name": movie.name,
                "posterLink": movie.posterLink,
                "genre": movie.genre,
                "premYear": parseInt(movie.premYear, 10),
                "time" : movie.time,
                "score" : parseFloat(movie.score),
                "description" : movie.description
            }
            console.log(self.updatedMovie)
            $http({
                    method: 'PUT',
                    url: 'https://localhost:44353/api/Movie/' + self.movieId,
                    data: self.updatedMovie,
                    headers: {
                        'Content-Type': 'application/json'
                    },
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