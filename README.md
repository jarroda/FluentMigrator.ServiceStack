FluentMigrator.ServiceStack [![Build status](https://ci.appveyor.com/api/projects/status/dk4m0kr0u7hj404g)](https://ci.appveyor.com/project/jarroda/fluentmigrator-servicestack)
===========================

FluentMigrator.ServiceStack is a [ServiceStack](https://servicestack.net/) plugin providing a dashboard UI for running [FluentMigrator](https://github.com/schambers/fluentmigrator) database migrations.

![](https://raw.githubusercontent.com/jarroda/FluentMigrator.ServiceStack/master/resources/Screenshot1.png)

Requirements
------------

There are separate ServiceStack V3 and V4 plugins.  FluentMigrator.Runner (and FluentMigrator) version >= 1.1.1.26 is required.

Installation
------------

The plugin is available via NuGet.  

[For ServiceStack V3:](https://www.nuget.org/packages/FluentMigrator.ServiceStack/)

```
    PM> Install-Package FluentMigrator.ServiceStack
```

[For ServiceStack V4:](https://www.nuget.org/packages/FluentMigrator.ServiceStack4/)

```
    PM> Install-Package FluentMigrator.ServiceStack4
```

Once the NuGet package is installed, enable the plugin in the AppHost Configure method.  The plugin requires a reference to the assembly containing the database migrations.

```csharp
    public override void Configure(Container container)
    {
        ...
        Plugins.Add(new MigrationFeature(typeof(TestMigrations.Mig_01).Assembly));
        ...
    }
```

Usage
------------

Once installed, the migration dashboard is available at:
(Assuming service hosted at /api)
```
http://localhost:port/api/migrations-ui/
```

Most of the FluentMigrator command line options are available via the GUI.  
- The migration mode defaults to Preview, allowing you to test your migrations without modifying the database.  This can be switched to Live to actually apply the migrations.
- The migration timeout can be selected from a dropdown list.
- The available migrations are listed in a table. Applied migrations appear in green along with their applied date.  The database version can be rolled forward or back to a specific migration by using the row-level 'Apply' buttons.  The database can be migrated all the way forward by selecting 'Apply All', and the latest migration can be rolled back with the 'Roll Back' button.
- The migration output appears in the 'Console Output' tray, along with a spinner when migrations are in progress.

Nuts & Bolts
------------

The core of the plugin wraps the FluentMigrator Runner in a RESTful API.  This API is consumed by an Angular JS web page, providing a very easy to use UI for running migrations.  All static resources (JS, HTML, images) are embedded in the plugin DLL and are served via ServiceStack, so there are no external dependencies.  The plugin is only active when running locally, so the dashboard is not accessible when running publicly (for obvious security reasons).