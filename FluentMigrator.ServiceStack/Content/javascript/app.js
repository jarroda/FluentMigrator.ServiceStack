var myApp = angular.module('myApp', ['ui.bootstrap', 'luegg.directives', 'ngSanitize']);


myApp.controller('MigrationCtrl', function ($scope, $http) {
    var baseUrl = '/migrations/'

    $scope.migrations = [];
    $scope.info = "";
    $scope.output = "";
    $scope.UnAppliedMigrationCount = 0;
    $scope.previewSelection = 1;

    $scope.timeOut = 5;

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.maxSize = 5;
    $scope.itemsPerPage = 15;
    $scope.totalItems = 0;
    $scope.currentPage = 1;

    $scope.appendOutput = function (data) {
        $scope.output += "<strong>This is a test</strong> of output appending and will be used to make the design work with variable data!\n";
        $scope.$apply();
    };

    $scope.updatePreviewSelection = function (bool) {
        $scope.previewSelection = bool;
    };


    $scope.getMigrations = function () {
        $http.get(baseUrl).
        success(function (data, status, headers, config) {
            var migrations = data.Migrations;

            $scope.info = data.Info;
            $scope.connectedDatabase = data.Database;
            $scope.migrations = migrations;

            $scope.totalItems = migrations.length;

            var AppliedCount = 0;
            for (k in migrations) {
                if (migrations[k]["AppliedOn"] === undefined) AppliedCount++;
            }
            $scope.UnAppliedMigrationCount = AppliedCount;
        }).
        error(function (data, status, headers, config) {
            console.log(data);
        });
    }

    $scope.ApplyMigration = function (migration) {
        var requestURL;

        if (migration != null)
        {
            requestURL = baseUrl + migration.Version;
        }
        else
        {
            var migration = {};
            requestURL = baseUrl;
        }

        migration.TimeOut = $scope.timeOut * 60;
        migration.PreviewOnly = $scope.previewSelection;

        console.log("Begining migration to " + ((migration.Version) ? "VERSION " + migration.Version : "HEAD" ) + " in " + ((migration.PreviewOnly) ? "PREVIEW" : "LIVE") + " mode with a " + migration.TimeOut + " second timeout.");

        $http.post(requestURL, migration).
        success(function (data, status, headers, config) {
            $scope.output = data;
            $scope.getMigrations();
        }).
        error(function (data, status, headers, config) {
            $scope.output = data;
            $scope.getMigrations();
        });
    }

    $scope.ClearOutput = function () {
        $scope.output = '';
    }

    $scope.getMigrations();
});

myApp.filter('dateFormat', function ($filter) {
    return function (utcDateString, format) {
        if(utcDateString != null)
        {
            date = new Date(parseInt(utcDateString.substr(6)));
            return $filter('date')(date, format);
        }
        else
        {
            return "";
        }
        
    };
});

myApp.filter('startFrom', function () {
    return function (input, start) {
        start = +start; //parse to int
        return input.slice(start);
    }
});