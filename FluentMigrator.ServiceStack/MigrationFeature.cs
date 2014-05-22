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
        private const string PathPrefix = "/migrations/ui";

        public MigrationFeature(Assembly migrationAssembly)
        {
            MigrationService.Assembly = migrationAssembly;
        }

        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<MigrationService>();

            appHost.CatchAllHandlers.Add((httpMethod, pathInfo, filePath) =>
            {
                if (pathInfo.StartsWith(PathPrefix))
                {                    
                    var path = pathInfo.Substring(PathPrefix.Length).TrimStart('/');

                    if (path.Length == 0)
                    {
                        path = "content/index.html";
                    }

                    return new EmbeddedFileHandler(path);
                }
                return null;
            });
        }
    }
}