var myApp = angular.module('myApp', ['ui.bootstrap', 'luegg.directives', 'ngSanitize']);


myApp.controller('MigrationCtrl', function ($scope, $http) {
    var baseUrl = window.location.pathname.replace('migrations-ui', 'migrations');

    $scope.migrations = [];
    $scope.info = "";
    $scope.output = "";
    $scope.UnAppliedMigrationCount = 0;
    $scope.previewSelection = 1;
    $scope.displayConnStringOption = 0;
    $scope.customDatabaseConnString = null;
    $scope.isBusy = false;
    $scope.timeOut = 5;
    $scope.maxSize = 5;
    $scope.itemsPerPage = 15;
    $scope.totalItems = 0;
    $scope.currentPage = 1;

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.appendOutput = function (data) {
        $scope.output += "<strong>This is a test</strong> of output appending and will be used to make the design work with variable data!\n";
        $scope.$apply();
    };

    $scope.updatePreviewSelection = function (bool) {
        $scope.previewSelection = bool;
    };
    $scope.showConnStringPanel = function () {
        $scope.displayConnStringOption = !$scope.displayConnStringOption;
    };
    $scope.applyCustomDatabaseConnString = function () {
        $scope.getMigrations();
    };

    $scope.getMigrations = function () {
        $http.get(baseUrl, { params: { connectionString: $scope.customDatabaseConnString } })
            .success(function (data, status, headers, config) {
                var migrations = data.Migrations;

                $scope.info = data.Info;
                $scope.connectedDatabase = data.Database;
                $scope.connectedDatabaseString = "Connected to <strong>" + data.Database + "</strong> database";

                $scope.migrations = data.Migrations;
                $scope.totalItems = data.Migrations.length;

                $scope.UnAppliedMigrationCount = data.Migrations.reduce(function (total, mig) {
                    return mig.AppliedOn ? total : total + 1;
                }, 0);
            })
            .error(function (data, status, headers, config) {
                console.log(data);
            });
    }

    $scope.ApplyMigration = function (migration, options) {        
        $scope.isBusy = true;
        $scope.output = "";
        var requestURL;

        if (migration != null) {
            requestURL = baseUrl + migration.Version;
        } else {
            var migration = {};
            requestURL = baseUrl;
        }

        var xhr = new XMLHttpRequest();
        xhr.onprogress = function () {
            $scope.output = xhr.responseText;
            $scope.$apply();
        }
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                $scope.output = xhr.responseText;
                $scope.isBusy = false;
                $scope.$apply();
                $scope.getMigrations();
            }
        }

        var params = "TimeoutSeconds=" + ($scope.timeOut * 60) + "&PreviewOnly=" + $scope.previewSelection;
        if($scope.customDatabaseConnString) params += "&ConnectionString=" + encodeURIComponent($scope.customDatabaseConnString);
        params = params.replace(/%20/g, '+');

        if (options == "rollback") {
            console.log("Begining ROLLBACK with params: " + params);
            xhr.open("DELETE", requestURL, true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.send(params);
        }
        else {
            console.log("Begining migration with params: " + params);
            xhr.open("POST", requestURL, true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.send(params);
        }
    }

    $scope.ClearOutput = function () {
        $scope.output = '';
    }

    $scope.getMigrations();
});

myApp.filter('dateFormat', function ($filter) {
    return function (utcDateString, format) {
        return utcDateString != null ? $filter('date')(new Date(parseInt(utcDateString.substr(6))), format) : "";
    };
});

myApp.filter('startFrom', function () {
    return function (input, start) {
        start = +start; //parse to int
        return input.slice(start);
    }
});