using Microsoft.EntityFrameworkCore.Migrations;

namespace neitek.Server.Migrations
{
    public partial class TareasImportante : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Importante",
                table: "Tareas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Importante",
                table: "Tareas");
        }
    }
}
