using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentMigrator.ServiceStack
{
    public sealed class MigrationFeature : IPlugin
    {
        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<MigrationService>();
        }
    }
}