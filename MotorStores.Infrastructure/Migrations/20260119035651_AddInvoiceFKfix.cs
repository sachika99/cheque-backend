using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorStores.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceFKfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Cheques_ChequeId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ChequeId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ChequeId1",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "ChequeId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ChequeId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChequeId1",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ChequeId1",
                table: "Invoices",
                column: "ChequeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Cheques_ChequeId1",
                table: "Invoices",
                column: "ChequeId1",
                principalTable: "Cheques",
                principalColumn: "Id");
        }
    }
}
