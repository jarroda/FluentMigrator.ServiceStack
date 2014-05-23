using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMigrations
{
    [Migration(2, "Here is a migration to add some indexes.")]
    public class Mig_02 : Migration
    {
        public override void Up()
        {
            Create.Index("ix_Name").OnTable("TestTable2").OnColumn("Name").Ascending()
               .WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("ix_Name").OnTable("TestTable2");
        }
    }
}
