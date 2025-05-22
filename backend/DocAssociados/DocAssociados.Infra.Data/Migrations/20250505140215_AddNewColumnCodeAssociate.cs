using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAssociados.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnCodeAssociate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Associado",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoRepresentante",
                table: "Associado",
                type: "varchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CodigoAssociado",
                table: "Associado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Associado_CodigoAssociado",
                table: "Associado",
                column: "CodigoAssociado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Associado_Cpf",
                table: "Associado",
                column: "Cpf",
                unique: true);

            migrationBuilder.Sql(
            "ALTER TABLE Associado MODIFY COLUMN CodigoAssociado INT NOT NULL AUTO_INCREMENT UNIQUE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Associado_CodigoAssociado",
                table: "Associado");

            migrationBuilder.DropIndex(
                name: "IX_Associado_Cpf",
                table: "Associado");

            migrationBuilder.DropColumn(
                name: "CodigoAssociado",
                table: "Associado");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Associado",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Associado",
                keyColumn: "CodigoRepresentante",
                keyValue: null,
                column: "CodigoRepresentante",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoRepresentante",
                table: "Associado",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4)",
                oldMaxLength: 4,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
