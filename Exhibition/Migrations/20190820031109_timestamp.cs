using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exhibition.Migrations
{
    public partial class timestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "editTime",
                table: "works",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "editTime",
                table: "works",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
