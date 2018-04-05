using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace XAcc.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Alter Database xacc COLLATE='utf8_general_ci'");

            migrationBuilder.CreateTable(
                name: "Stmas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    sellpr1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    stkcod = table.Column<string>(nullable: true),
                    stkdes = table.Column<string>(nullable: true),
                    stkdes2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stmas", x => x.ID);
                });
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stmas");
        }
    }
}
