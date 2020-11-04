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