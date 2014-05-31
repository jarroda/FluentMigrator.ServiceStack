using System.Collections.Generic;
using ServiceStack;
#if V3
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
#endif

namespace FluentMigrator.ServiceStack.ServiceModel
{
    public sealed class MigrationInfoResponse : IHasResponseStatus
    {
        public List<MigrationInfo> Migrations { get; set; }

        public string Info { get; set; }

        public ResponseStatus ResponseStatus { get; set; }

        public string Database { get; set; }
    }
}