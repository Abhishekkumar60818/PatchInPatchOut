using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatchInPatchOut.Migrations
{
    /// <inheritdoc />
    public partial class mg2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ShiftEnd",
                table: "Users",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ShiftStart",
                table: "Users",
                type: "time",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "ShiftEnd", "ShiftStart" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftEnd",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShiftStart",
                table: "Users");
        }
    }
}
