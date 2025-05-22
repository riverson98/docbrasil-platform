using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAssociados.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnToProfilePhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ComprovanteDeResidenciaUpload",
                table: "Endereco",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FotoDePerfilUrl",
                table: "Associado",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoDePerfilUrl",
                table: "Associado");

            migrationBuilder.UpdateData(
                table: "Endereco",
                keyColumn: "ComprovanteDeResidenciaUpload",
                keyValue: null,
                column: "ComprovanteDeResidenciaUpload",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ComprovanteDeResidenciaUpload",
                table: "Endereco",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
