using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorStores.Infrastructure.Migrations
{
    public partial class fixingbankacountunqewithbankid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_UserId_AccountNo",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserIds");

            migrationBuilder.RenameColumn(
                name: "createdBy",
                table: "UserIds",
                newName: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UserId_AccountNo_BankId",
                table: "BankAccounts",
                columns: new[] { "UserId", "AccountNo", "BankId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_UserId_AccountNo_BankId",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "UserIds",
                newName: "createdBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserIds",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UserId_AccountNo",
                table: "BankAccounts",
                columns: new[] { "UserId", "AccountNo" },
                unique: true);
        }
    }
}
