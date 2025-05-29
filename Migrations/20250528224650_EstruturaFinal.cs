using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capylender.API.Migrations
{
    /// <inheritdoc />
    public partial class EstruturaFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfissionalServicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfissionalServicos",
                columns: table => new
                {
                    ProfissionaisId = table.Column<int>(type: "int", nullable: false),
                    ServicosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalServicos", x => new { x.ProfissionaisId, x.ServicosId });
                    table.ForeignKey(
                        name: "FK_ProfissionalServicos_Profissionais_ProfissionaisId",
                        column: x => x.ProfissionaisId,
                        principalTable: "Profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfissionalServicos_Servicos_ServicosId",
                        column: x => x.ServicosId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalServicos_ServicosId",
                table: "ProfissionalServicos",
                column: "ServicosId");
        }
    }
}
