using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Migrations
{
    public partial class Modify_Convert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "File2",
                table: "ConvertAddress",
                newName: "OutPut");

            migrationBuilder.RenameColumn(
                name: "File1",
                table: "ConvertAddress",
                newName: "Input2");

            migrationBuilder.AddColumn<string>(
                name: "Input1",
                table: "ConvertAddress",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "ConvertAddress",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Input1",
                table: "ConvertAddress");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "ConvertAddress");

            migrationBuilder.RenameColumn(
                name: "OutPut",
                table: "ConvertAddress",
                newName: "File2");

            migrationBuilder.RenameColumn(
                name: "Input2",
                table: "ConvertAddress",
                newName: "File1");
        }
    }
}
