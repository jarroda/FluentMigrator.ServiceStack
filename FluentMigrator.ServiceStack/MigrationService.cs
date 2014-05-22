using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.ServiceStack.ServiceModel;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FluentMigrator.ServiceStack
{
    [Restrict(LocalhostOnly=true)]
    public class MigrationService : Service
    {
        internal static Assembly Assembly { get; set; }

        public IDbConnectionFactory Database { get; set; }

        public object Get(VersionInfo request)
        {
            var availableMigrations = Assembly.GetTypes()
                .Where(t => t.IsDefined(typeof(MigrationAttribute), true))
                .Select(t =>
                {
                    var m = (MigrationAttribute)t.GetCustomAttributes(typeof(MigrationAttribute), true).First();
                    return new VersionInfo 
                    { 
                        Description = m.Description ?? t.Name, 
                        Version = m.Version,
                        IsAvailable = true,
                    };
                })
                .ToList();

            using(var con = Database.OpenDbConnection())
            {
                List<VersionInfo> appliedMigrations;

                if (con.TableExists("VersionInfo"))
                {
                    appliedMigrations = con.Select<VersionInfo>();
                }
                else
                {
                    appliedMigrations = new List<VersionInfo>();
                }

                return new VersionInfoResponse
                {
                    Migrations = appliedMigrations
                        .Concat(availableMigrations)
                        .ToLookup(v => v.Version)
                        .Select(g => g.Aggregate((v1, v2) => new VersionInfo
                        {
                            Version = v1.Version,
                            Description = v1.Description ?? v2.Description,
                            AppliedOn = v1.AppliedOn ?? v2.AppliedOn,
                            IsAvailable = v1.IsAvailable || v2.IsAvailable,
                        }))
                        .OrderByDescending(v => v.Version)
                        .ToList(),
                    Info = appliedMigrations.Except(availableMigrations).Any() ? "Warning: Database has migrations applied that are not available in the current migration assembly.  Rollback may not be possible." : null,
                    Database = con.Database,
                };
            }            
        }

        public object Post(Migrate request)
        {
            //var writer = new StreamWriter(Response.OutputStream) { AutoFlush = true };
            var writer = new StringWriter();
            
            Migrate(writer, request.PreviewOnly, false, request.Version);

            writer.Flush();

            var s = writer.ToString();
            return s.ToString();
        }

        public void Delete(Migrate request)
        {
            var writer = new StreamWriter(Response.OutputStream) { AutoFlush = true };

            Migrate(writer, request.PreviewOnly, true, request.Version);

            writer.Flush();
        }

        private void Migrate(TextWriter writer, bool previewOnly, bool rollback, long? version = null)
        {
            string connectionString;
            long highestMigration;
            string task, database;

            using (var con = Database.OpenDbConnection())
            {
                database = DialectToDatabaseString(con.GetDialectProvider());
                connectionString = con.ConnectionString;
                highestMigration = con.TableExists("VersionInfo") ? con.Scalar<long>("SELECT MAX(Version) from VersionInfo") : -1;
            }            

            if (rollback)
            {
                task = version.HasValue ? "rollback:toversion" : "rollback";
            }
            else
            {
                task = !version.HasValue || version.Value >= highestMigration ? "migrate:up" : "migrate:down";
            }

            var announcer = new TextWriterAnnouncer(writer)
            {
                ShowElapsedTime = true,
                ShowSql = true,
            };

            var context = new RunnerContext(announcer)
            {
                Database = database,
                Connection = connectionString,
                Target = Assembly.Location,
                PreviewOnly = previewOnly,
                Task = task,
                Version = version ?? 0,
            };

            new TaskExecutor(context).Execute();
        }

        private string DialectToDatabaseString(IOrmLiteDialectProvider provider)
        {
            switch (provider.GetType().Name)
            {
                case "SqliteOrmLiteDialectProvider":
                    return "sqlite";
                default:
                    return "sqlserver";
            }
        }
    }
}