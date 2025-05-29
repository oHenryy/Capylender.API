using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capylender.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Bloqueado", "ClienteId", "Email", "Nome", "ProfissionalId", "Role", "SenhaHash" },
                values: new object[] { 1, false, null, "admin@capylender.com", "Administrador", null, "Admin", "$2a$11$adlkUYw1ZZB17CC6tqWXEuqaK4QWewvsYe8ZI3fiRwG.QjaM3QzEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
