using ServiceStack;
#if V3
using ServiceStack.ServiceHost;
#endif

namespace FluentMigrator.ServiceStack.ServiceModel
{
    [Route("/migrations", "GET")]
    public class GetMigrations
    {
        public string ConnectionString { get; set; }
    }
}