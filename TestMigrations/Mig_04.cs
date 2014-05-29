using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestMigrations
{
    [Migration(4, "Long migration")]
    public class Mig_04 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("TestTable").Row(new { Name = "Test1" });
            Thread.Sleep(1000);
            Insert.IntoTable("TestTable").Row(new { Name = "Test2" });
            Thread.Sleep(1000);
            Insert.IntoTable("TestTable").Row(new { Name = "Test3" });
            Thread.Sleep(1000);
            Insert.IntoTable("TestTable").Row(new { Name = "Test4" });
            Thread.Sleep(1000);
            Insert.IntoTable("TestTable").Row(new { Name = "Test5" });
        }

        public override void Down()
        {

        }
    }
}
