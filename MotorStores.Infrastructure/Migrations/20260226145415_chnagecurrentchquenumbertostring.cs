using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorStores.Infrastructure.Migrations
{
    public partial class chnagecurrentchquenumbertostring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_BankAccountId_ChequeNo",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_Cheque_Books_BankAccountId_ChequeBookNo",
                table: "Cheque_Books");

            migrationBuilder.DropIndex(
                name: "IX_Banks_BranchCode",
                table: "Banks");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentChequeNo",
                table: "Cheque_Books",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BankAccountId_ChequeNo_UserId",
                table: "Cheques",
                columns: new[] { "BankAccountId", "ChequeNo", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cheque_Books_BankAccountId_ChequeBookNo_UserId",
                table: "Cheque_Books",
                columns: new[] { "BankAccountId", "ChequeBookNo", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_BranchCode_UserId",
                table: "Banks",
                columns: new[] { "BranchCode", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_BankAccountId_ChequeNo_UserId",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_Cheque_Books_BankAccountId_ChequeBookNo_UserId",
                table: "Cheque_Books");

            migrationBuilder.DropIndex(
                name: "IX_Banks_BranchCode_UserId",
                table: "Banks");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentChequeNo",
                table: "Cheque_Books",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BankAccountId_ChequeNo",
                table: "Cheques",
                columns: new[] { "BankAccountId", "ChequeNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cheque_Books_BankAccountId_ChequeBookNo",
                table: "Cheque_Books",
                columns: new[] { "BankAccountId", "ChequeBookNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_BranchCode",
                table: "Banks",
                column: "BranchCode",
                unique: true);
        }
    }
}
