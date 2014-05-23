using ServiceStack.ServiceInterface.ServiceModel;
using System.Collections.Generic;

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