using ServiceStack;
using ServiceStack.Common.Web;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FluentMigrator.ServiceStack
{
    public sealed class MigrationFeature : IPlugin
    {
        public MigrationFeature(Assembly migrationAssembly)
        {
            MigrationService.Assembly = migrationAssembly;
        }

        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<MigrationService>();

            //appHost.CatchAllHandlers.Add(new HttpHandlerResolverDelegate(
            appHost.CatchAllHandlers.Add((httpMethod, pathInfo, filePath) =>
            {
                if (pathInfo == "/migrations/ui" || pathInfo == "/migrations/ui/" || pathInfo == "/migrations/ui/default.html")
                {
                    
                    //return new EndpointHandlerBase
                    //return new StaticFileHandler();


                    var indexFile = appHost.VirtualPathProvider.GetFile("/bin/Content/index.html");
                    if (indexFile != null)
                    {
                        return new VirtualFileHandler(indexFile);
                    }
                }
                return null;
            });
        }
    }
}