(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorAddCtrl', ['$http', '$state', ActorAddCtrl]);

    let sleep = ms => new Promise(resolve => setTimeout(resolve, ms));

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
                url: 'http://localhost:8080/api/actor',
                data: self.updatedActor,
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(function(response) {
                console.log('succes');
                sleep(1000);
                $state.go('actors');
            }, function(error) {
                console.log('can not put data.');
            });
        };
    };
}());