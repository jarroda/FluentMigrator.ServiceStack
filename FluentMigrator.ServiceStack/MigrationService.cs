using FluentMigrator;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.ServiceStack.ServiceModel;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.Service;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluentMigrator.ServiceStack
{
    [Restrict(LocalhostOnly=true)]
    public class MigrationService : Service
    {
        private static MigrationComparer _comparer = new MigrationComparer();

        public IDbConnectionFactory Database { get; set; }

        public object Get(VersionInfo request)
        {
            var availableMigrations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsDefined(typeof(MigrationAttribute), true))
                .Select(t =>
                {
                    var m = (MigrationAttribute)t.GetCustomAttributes(typeof(MigrationAttribute), true).First();
                    return new VersionInfo { Description = m.Description ?? t.Name, Version = m.Version };
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
                    AppliedMigrations = appliedMigrations,
                    AvailableMigrations = availableMigrations,
                    Info = appliedMigrations.Except(availableMigrations).Any() ? "Warning: Database has migrations applied that are not available in the current migration assembly.  Rollback may not be possible." : string.Empty,
                };
            }            
        }

        public void Post(Migrate request)
        {
            var writer = new StreamWriter(Response.OutputStream) { AutoFlush = true };
            
            Migrate(writer, request.PreviewOnly, false, request.Version);

            writer.Flush();
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
            string task;

            // Is there a better way of getting the connection string without opening up a connection?
            using (var con = Database.OpenDbConnection())
            {
                connectionString = con.ConnectionString;
                highestMigration = con.Scalar<long>("SELECT MAX(Version) from VersionInfo");
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
                Database = "sqlserver",
                Connection = connectionString,
                Target = Assembly.GetExecutingAssembly().Location,
                PreviewOnly = previewOnly,
                Task = task,
                Version = version ?? 0,
            };

            new TaskExecutor(context).Execute();
        }

        private class MigrationComparer : IEqualityComparer<VersionInfo>
        {
            public bool Equals(VersionInfo x, VersionInfo y)
            {
                return x.Version == y.Version;
            }

            public int GetHashCode(VersionInfo obj)
            {
                return obj.Version.GetHashCode();
            }
        }
    }
}