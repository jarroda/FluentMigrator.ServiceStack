using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMigrations
{
    [Migration(4)]
    public class Mig_04 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("TestTable").Row(new { Name = "Test" });
        }

        public override void Down()
        {

        }
    }
}
