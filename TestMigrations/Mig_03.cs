using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMigrations
{
    [Migration(3)]
    public class Mig_03 : Migration
    {
        public override void Up()
        {
            Create.Column("Name2").OnTable("TestTable2").AsBoolean().Nullable();

            Create.ForeignKey("fk_TestTable2_TestTableId_TestTable_Id")
                .FromTable("TestTable2").ForeignColumn("TestTableId")
                .ToTable("TestTable").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_TestTable2_TestTableId_TestTable_Id").OnTable("TestTable2");

            // Can't delete columns in Sqlite.
            Execute.Sql(@"
            CREATE TEMPORARY TABLE t1_backup(Id,Name,TestTableId);
            INSERT INTO t1_backup SELECT Id,Name,TestTableId FROM TestTable2;
            DROP TABLE TestTable2;
            CREATE TABLE TestTable2(Id,Name,TestTableId);
            INSERT INTO TestTable2 SELECT Id,Name,TestTableId FROM t1_backup;
            DROP TABLE t1_backup;");

            // Re-create index.
            Create.Index("ix_Name").OnTable("TestTable2").OnColumn("Name").Ascending()
                .WithOptions().NonClustered();
        }
    }
}
