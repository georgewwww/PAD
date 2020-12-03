(function() {
    'use strict';

    angular
        .module('hr')
        .controller('MovieAddCtrl', ['$http', '$state', MovieAddCtrl]);

    function MovieAddCtrl($http, $state, ) {
        var self = this;

        self.upload = function(movie) {
            console.log(movie)
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
            $http({
                    method: 'POST',
                    url: 'http://localhost:8080/api/movie',
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