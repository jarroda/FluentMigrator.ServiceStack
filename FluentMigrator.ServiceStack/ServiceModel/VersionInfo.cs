using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;
using System;

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Route("/migrations", "GET")]
    public class VersionInfo : IReturn<VersionInfoResponse>
    {
        public long? Version { get; set; }

        public DateTime? AppliedOn { get; set; }

        public string Description { get; set; }
    }
}