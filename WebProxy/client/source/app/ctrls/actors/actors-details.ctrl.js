(function() {
    'use strict';

    angular
        .module('hr')
        .controller('ActorDetailsCtrl', ['$http', '$stateParams', '$state', ActorDetailsCtrl]);

    let sleep = ms => new Promise(resolve => setTimeout(resolve, ms));

    function ActorDetailsCtrl($http, $stateParams, $state) {
        var self = this;

        self.actorId = $stateParams.actorId;

        $http({
            method: 'get',
            url: 'http://localhost:8080/api/Actor/' + self.actorId
        }).then(function(response) {
            console.log(response, 'res');
            self.actor = response.data;
        }, function(error) {
            console.log(error, 'can not get data.');
        });

        self.delete = function(actorId) {
            $http({
                method: 'DELETE',
                url: 'http://localhost:8080/api/Actor?Id=' + actorId
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