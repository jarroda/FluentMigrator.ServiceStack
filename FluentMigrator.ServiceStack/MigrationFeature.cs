using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluentMigrator.ServiceStack
{
    public sealed class MigrationFeature : IPlugin
    {
        internal static Assembly[] Assemblies { get; set; }

        public MigrationFeature(params Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<MigrationService>();
        }
    }
}