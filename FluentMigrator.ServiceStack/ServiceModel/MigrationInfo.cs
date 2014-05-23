using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;
using System;

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Alias("VersionInfo")]
    public class MigrationInfo : IReturn<MigrationInfoResponse>
    {
        public long? Version { get; set; }

        public DateTime? AppliedOn { get; set; }

        public string Description { get; set; }

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