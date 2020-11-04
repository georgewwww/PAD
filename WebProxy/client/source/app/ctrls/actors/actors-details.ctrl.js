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