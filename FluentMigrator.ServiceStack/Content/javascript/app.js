var myApp = angular.module('myApp', ['ui.bootstrap', 'luegg.directives', 'ngSanitize']);


myApp.controller('MigrationCtrl', function ($scope, $http) {
    var baseUrl = 'http://localhost:62303/migrations/'

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
        delete migrations;
        var migrations = {};

        $http.get(baseUrl).
        success(function (data, status, headers, config) {
            // this callback will be called asynchronously
            // when the response is available

            console.log(data);

            $scope.info = data.Info;

            //for (var i = 0, len = data.AppliedMigrations.length; i < len; i++) {
            //    data.AppliedMigrations[i].avaliable = false;
            //    data.AppliedMigrations[i].applied = true;
            //    migrations[data.AppliedMigrations[i].Version] = data.AppliedMigrations[i];
            //}
            //for (var i = 0, len = data.AvailableMigrations.length; i < len; i++) {
            //    if (migrations[data.AvailableMigrations[i].Version]) {
            //        migrations[data.AvailableMigrations[i].Version].avaliable = true;

            //        if (!migrations[data.AvailableMigrations[i].Version].Description) {
            //            migrations[data.AvailableMigrations[i].Version].Description = data.AvailableMigrations[i].Description;
            //        }
            //    }
            //    else {
            //        data.AvailableMigrations[i].avaliable = true;
            //        data.AvailableMigrations[i].applied = false;
            //        migrations[data.AvailableMigrations[i].Version] = data.AvailableMigrations[i];
            //    }
            //}
            //for (var k in migrations) {
            //    $scope.migrations.push(migrations[k]);
            //}

            $scope.migrations = data.Migrations;

            $scope.totalItems = data.Migrations.length;

            $scope.UnAppliedMigrationCount = data.Migrations.reduce(function (total, mig) {
                return mig.AppliedOn ? total + 1 : total
            }, 0);
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