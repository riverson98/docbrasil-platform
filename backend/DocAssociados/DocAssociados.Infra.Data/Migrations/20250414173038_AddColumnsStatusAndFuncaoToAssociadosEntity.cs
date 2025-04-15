using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAssociados.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsStatusAndFuncaoToAssociadosEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Funcao",
                table: "Associado",
                type: "int",
                maxLength: 1,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Associado",
                type: "int",
                maxLength: 1,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Funcao",
                table: "Associado");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Associado");
        }
    }
}
