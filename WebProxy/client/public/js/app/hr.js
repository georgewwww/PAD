angular.module('hr', [
    'ui.router'
]);
(function() {
    'use strict';

    angular
        .module('hr')
        .config(['$stateProvider', '$urlRouterProvider', routes]);

    function routes($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('movies', {
                url: '/movies',
                templateUrl: '/public/templates/movies/movies.tpl.html',
                controller: 'MovieCtrl',
                controllerAs: 'movie'
            })
            .state('edit', {
                url: '/movies/edit/:movieId',
                templateUrl: '/public/templates/movies/movies-edit.tpl.html',
                controller: 'MovieEditCtrl',
                controllerAs: 'm'
            })
            .state('upload', {
                url: '/movies/upload',
                templateUrl: '/public/templates/movies/movies-add.tpl.html',
                controller: 'MovieAddCtrl',
                controllerAs: 'adm'
            })
            .state('acupload', {
                url: '/actors/upload',
                templateUrl: '/public/templates/actors/actors-add.tpl.html',
                controller: 'ActorAddCtrl',
                controllerAs: 'adc'
            })
            .state('acedit', {
                url: '/actors/edit/:actorId',
                templateUrl: '/public/templates/actors/actors-edit.tpl.html',
                controller: 'ActorEditCtrl',
                controllerAs: 'a'
            })
            .state('actors', {
                url: '/actors',
                templateUrl: '/public/templates/actors/actors.tpl.html',
                controller: 'ActorCtrl',
                controllerAs: 'actor'
            })
            .state('adetails', {
                url: '/actors/details/:actorId',
                templateUrl: '/public/templates/actors/actors-details.tpl.html',
                controller: 'ActorDetailsCtrl',
                controllerAs: 'ad'
            })
            .state('mdetails', {
                url: '/movies/details/:movieId',
                templateUrl: '/public/templates/movies/movies-details.tpl.html',
                controller: 'MovieDetailsCtrl',
                controllerAs: 'md'
            });

        $urlRouterProvider.otherwise('/movies');
    }
}());

(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorAddCtrl', ['$http', '$state', ActorAddCtrl]);

    function ActorAddCtrl($http, $state, ) {
        var self = this;

        self.upload = function(actor) {
            console.log(actor)

            self.updatedActor = {
                "id": actor.id,
                "fullName": actor.fullName,
                "imageLink": actor.imageLink,
                "birthYear": parseInt(actor.birthYear, 10),
                "birthDate": actor.birthDate,
                "description": actor.description
            }

            $http({
                method: 'POST',
                url: 'https://localhost:44353/api/actor',
                data: self.updatedActor,
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(function(response) {
                console.log('succes');
                $state.go('actors');
            }, function(error) {
                console.log('can not put data.');
            });
        };
    };
}());

(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorEditCtrl', ['$http', '$stateParams', '$state', ActorEditCtrl]);

    function ActorEditCtrl($http, $stateParams, $state) {
        var self = this;

        console.log($stateParams.actorId)
        self.actorId = $stateParams.actorId

        $http({
            method: 'get',
            url: 'https://localhost:44353/api/Actor/' + self.actorId
        }).then(function(response) {
            console.log(response, 'res');
            self.actor = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });

        self.update = function(actor) {
            self.updatedActor = {
                "id": actor.id,
                "fullName": actor.fullName,
                "imageLink": actor.imageLink,
                "birthYear": parseInt(actor.birthYear, 10),
                "birthDate": actor.birthDate,
                "description": actor.description
            }
            console.log(self.updatedActor)
            $http({
                    method: 'PUT',
                    url: 'https://localhost:44353/api/Actor/' + self.actorId,
                    data: self.updatedActor,
                    headers: {
                        'Content-Type': 'application/json'
                    },
                })
                .then(function(response) {
                    console.log('succes');
                    $state.go('actors');
                }, function(error) {
                    console.log('can not put data.');
                });
        };
    };
}());

(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorCtrl', ['$http', ActorCtrl]);

    function ActorCtrl($http) {
        var self = this;
        $http({
            method: 'GET',
            url: 'https://localhost:44353/api/actor',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then(function(response) {
            console.log(response, 'res');
            self.actors = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });
    }
}());

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
                    url: 'https://localhost:44353/api/movie',
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

(function() {
    'use strict';

    angular
        .module('hr')
        .controller('MovieCtrl', ['$http', MovieCtrl]);

    function MovieCtrl($http) {
        var self = this;

        $http({
            method: 'get',
            url: 'https://localhost:44353/api/movie'
        }).then(function(response) {
            console.log(response, 'res');
            self.movies = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });
    };
}());

(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorDetailsCtrl', ['$http', '$stateParams', '$state', ActorDetailsCtrl]);

    function ActorDetailsCtrl($http, $stateParams, $state) {
        var self = this;

        self.actorId = $stateParams.actorId;
        
        $http({
            method: 'get',
            url: 'https://localhost:44353/api/Actor/' + self.actorId
        }).then(function(response) {
            console.log(response, 'res');
            self.actor = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });

        self.delete = function(actorId) {
            $http({
                method: 'DELETE',
                url: 'https://localhost:44353/api/Actor?Id=' + actorId
            })
            .then(function(response) {
                console.log('succes');
                $state.go('actors');
            }, function(error) {
                console.log('can not put data.');
            });
        };
    };
}());

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