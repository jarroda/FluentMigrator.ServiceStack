using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.ServiceStack.ServiceModel;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentMigrator.ServiceStack
{
    [Restrict(LocalhostOnly=true)]
    public class MigrationService : Service
    {
        internal static Assembly Assembly { get; set; }

        public IDbConnectionFactory Database { get; set; }

        public object Get(GetMigrations request)
        {
            var availableMigrations = Assembly.GetTypes()
                .Where(t => t.IsDefined(typeof(MigrationAttribute), true))
                .Select(t =>
                {
                    var m = (MigrationAttribute)t.GetCustomAttributes(typeof(MigrationAttribute), true).First();
                    return new MigrationInfo 
                    { 
                        Description = m.Description ?? t.Name, 
                        Version = m.Version,
                        IsAvailable = true,
                    };
                })
                .ToList();

            using (var con = string.IsNullOrEmpty(request.ConnectionString) ? Database.OpenDbConnection() : OpenConnection(request.ConnectionString))
            {
                List<MigrationInfo> appliedMigrations;

                if (con.TableExists("VersionInfo"))
                {
                    appliedMigrations = con.Select<MigrationInfo>();
                }
                else
                {
                    appliedMigrations = new List<MigrationInfo>();
                }

                return new MigrationInfoResponse
                {
                    Migrations = appliedMigrations
                        .Concat(availableMigrations)
                        .ToLookup(v => v.Version)
                        .Select(g => g.Aggregate((v1, v2) => new MigrationInfo
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

        public void Post(Migrate request)
        {
            using (var writer = Response.GetChunkedWriter())
            {
                Migrate(writer, request.PreviewOnly, false, request.Version, request.ConnectionString);
            }

            Response.Close();
        }

        public void Delete(Migrate request)
        {
            using (var writer = Response.GetChunkedWriter())
            {
                Migrate(writer, request.PreviewOnly, true, request.Version, request.ConnectionString);
            }

            Response.Close();
        }

        private void Migrate(TextWriter writer, bool previewOnly, bool rollback, long? version = null, string connectionString = null)
        {
            string cs;
            long highestMigration;
            string task, database;

            using (var con = string.IsNullOrEmpty(connectionString) ? Database.OpenDbConnection() : OpenConnection(connectionString))
            {
                database = DialectToDatabaseString(con.GetDialectProvider());
                cs = con.ConnectionString;
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
                Connection = cs,
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

        private IDbConnection OpenConnection(string connectionString)
        {
            var con = OrmLiteConfig.ToDbConnection(connectionString);
            con.Open();
            return con;
        }
    }
}