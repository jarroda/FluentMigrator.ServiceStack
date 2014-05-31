using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
#if V3
using ServiceStack.ServiceHost;
#endif

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Alias("VersionInfo")]
    public class MigrationInfo : IReturn<MigrationInfoResponse>
    {
        public long? Version { get; set; }

        public DateTime? AppliedOn { get; set; }

        public string Description { get; set; }

        [Ignore]
        public bool IsAvailable { get; set; }

        public override bool Equals(object obj)
        {
            return Version == ((MigrationInfo)obj).Version;
        }

        public override int GetHashCode()
        {
            return (Version ?? 0).GetHashCode();
        }
    }
}