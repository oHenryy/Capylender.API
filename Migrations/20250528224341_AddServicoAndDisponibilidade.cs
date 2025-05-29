using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capylender.API.Migrations
{
    /// <inheritdoc />
    public partial class AddServicoAndDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Clientes_ClienteId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "Duracao",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DiaSemana",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "HoraFim",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "Disponibilidades",
                newName: "Disponivel");

            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                table: "Agendamentos",
                newName: "DataConfirmacao");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Servicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuracaoMinutos",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfissionalId",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Disponibilidades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Disponibilidades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProfissionalId1",
                table: "Disponibilidades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Clientes",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Cancelado",
                table: "Agendamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmado",
                table: "Agendamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCancelamento",
                table: "Agendamentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfissionalId1",
                table: "Agendamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ProfissionalId",
                table: "Servicos",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilidades_ProfissionalId1",
                table: "Disponibilidades",
                column: "ProfissionalId1");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CPF",
                table: "Clientes",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ProfissionalId1",
                table: "Agendamentos",
                column: "ProfissionalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Clientes_ClienteId",
                table: "Agendamentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId",
                table: "Agendamentos",
                column: "ProfissionalId",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId1",
                table: "Agendamentos",
                column: "ProfissionalId1",
                principalTable: "Profissionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId",
                table: "Disponibilidades",
                column: "ProfissionalId",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId1",
                table: "Disponibilidades",
                column: "ProfissionalId1",
                principalTable: "Profissionais",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Profissionais_ProfissionalId",
                table: "Servicos",
                column: "ProfissionalId",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Clientes_ClienteId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId1",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId",
                table: "Disponibilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId1",
                table: "Disponibilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Profissionais_ProfissionalId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_ProfissionalId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Disponibilidades_ProfissionalId1",
                table: "Disponibilidades");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CPF",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ProfissionalId1",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ProfissionalId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "ProfissionalId1",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Cancelado",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "Confirmado",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DataCancelamento",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "ProfissionalId1",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "Disponivel",
                table: "Disponibilidades",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "DataConfirmacao",
                table: "Agendamentos",
                newName: "DataAtualizacao");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Servicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duracao",
                table: "Servicos",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "DiaSemana",
                table: "Disponibilidades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraFim",
                table: "Disponibilidades",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicio",
                table: "Disponibilidades",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Clientes_ClienteId",
                table: "Agendamentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Profissionais_ProfissionalId",
                table: "Agendamentos",
                column: "ProfissionalId",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Profissionais_ProfissionalId",
                table: "Disponibilidades",
                column: "ProfissionalId",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
