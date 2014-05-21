using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.OrmLite;
using System.Data.SQLite;
using System.IO;

namespace FluentMigrator.ServiceStack.TestV3
{
    [Route("/hello")]
    [Route("/hello/{Name}")]
    public class Hello
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    public class HelloService : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, " + request.Name };
        }
    } 

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public class HelloAppHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public HelloAppHost() : base("Hello Web Services", typeof(HelloService).Assembly) { }

            public override void Configure(Funq.Container container)
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "test.db");
                File.Delete(dbPath);

                SQLiteConnection.CreateFile(dbPath);
                Plugins.Add(new MigrationFeature(typeof(TestMigrations.Mig_01).Assembly));

                container.Register<IDbConnectionFactory>(new OrmLiteConnectionFactory("Data Source=" + dbPath + ";Version=3;", false, SqliteDialect.Provider));
            }
        }

        protected void Application_Start()
        {
            new HelloAppHost().Init();
        }
    }
}