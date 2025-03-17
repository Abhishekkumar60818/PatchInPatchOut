using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatchInPatchOut.Migrations
{
    /// <inheritdoc />
    public partial class addosme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpacingShiftIn",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpacingShiftOut",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "SpacingShiftIn", "SpacingShiftOut" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpacingShiftIn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpacingShiftOut",
                table: "Users");
        }
    }
}
