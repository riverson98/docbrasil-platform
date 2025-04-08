using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAssociados.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsToTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FichaAssociacaoUploadUrl",
                table: "Associado",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TermoDeAdessaoUploadUrl",
                table: "Associado",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FichaAssociacaoUploadUrl",
                table: "Associado");

            migrationBuilder.DropColumn(
                name: "TermoDeAdessaoUploadUrl",
                table: "Associado");
        }
    }
}
