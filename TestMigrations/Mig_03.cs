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
            Delete.Column("Name2").FromTable("TestTable2");
        }
    }
}
