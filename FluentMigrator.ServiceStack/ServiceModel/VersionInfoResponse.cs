using ServiceStack.ServiceInterface.ServiceModel;
using System.Collections.Generic;

namespace FluentMigrator.ServiceStack.ServiceModel
{
    public sealed class VersionInfoResponse : IHasResponseStatus
    {
        public List<VersionInfo> Migrations { get; set; }

        public string Info { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }
}