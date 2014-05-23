using ServiceStack.ServiceHost;

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Route("/migrations", "GET")]
    public class GetMigrations
    {
        public string ConnectionString { get; set; }
    }
}
