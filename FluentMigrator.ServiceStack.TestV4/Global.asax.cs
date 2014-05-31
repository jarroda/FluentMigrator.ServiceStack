using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack.OrmLite;
using System.Data.SQLite;
using System.IO;
using ServiceStack;
using ServiceStack.Data;

namespace FluentMigrator.ServiceStack.TestV4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public class AppHost : AppHostBase
        {
            public AppHost() : base("Fluent Migrator Web Services", typeof(AppHost).Assembly) { }

            public override void Configure(Funq.Container container)
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "test.db");

                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }

                Plugins.Add(new CorsFeature());
                Plugins.Add(new PostmanFeature());
                Plugins.Add(new MigrationFeature(typeof(TestMigrations.Mig_01).Assembly));
                
                container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory("Data Source=" + dbPath + ";Version=3;", SqliteDialect.Provider));
            }
        }

        protected void Application_Start()
        {
            new AppHost().Init();
        }
    }
}