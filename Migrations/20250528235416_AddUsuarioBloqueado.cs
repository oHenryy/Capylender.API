using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capylender.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioBloqueado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Bloqueado",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bloqueado",
                table: "Usuarios");
        }
    }
}
