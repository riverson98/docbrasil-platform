using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAssociados.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class addColumnToRequerimentoJudicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CodigoAssociado",
                table: "Associado",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequerimentoJudicialUrl",
                table: "Associado",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequerimentoJudicialUrl",
                table: "Associado");

            migrationBuilder.AlterColumn<int>(
                name: "CodigoAssociado",
                table: "Associado",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
