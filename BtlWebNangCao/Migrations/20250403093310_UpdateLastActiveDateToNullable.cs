using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BtlWebNangCao.Migrations
{
    public partial class UpdateLastActiveDateToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveDate",
                table: "AspNetUsers");
        }
    }
}
