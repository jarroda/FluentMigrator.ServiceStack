using ServiceStack.ServiceHost;

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Route("/migrations", "POST, DELETE")]
    [Route("/migrations/{Version}", "POST, DELETE")]
    public sealed class Migrate
    {
        public long? Version { get; set; }

        public bool PreviewOnly { get; set; }
    }
}