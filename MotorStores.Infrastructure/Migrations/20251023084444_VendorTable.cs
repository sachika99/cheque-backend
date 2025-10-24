using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorStores.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VendorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Unique vendor code identifier"),
                    VendorName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Full name of the vendor/supplier"),
                    VendorAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Physical address of the vendor"),
                    VendorPhoneNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Contact phone number"),
                    VendorEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Contact email address"),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Bank name for payment processing"),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Bank account number for payments"),
                    CrediPeriodDays = table.Column<int>(type: "int", nullable: true, comment: "Credit period in days"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Current status of the vendor"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Additional notes about the vendor"),
                    ContactPerson = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Contact person name"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Record creation timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Record last update timestamp"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "User who created the record"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "User who last updated the record")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Status",
                table: "Vendors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorCode",
                table: "Vendors",
                column: "VendorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorEmail",
                table: "Vendors",
                column: "VendorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorName",
                table: "Vendors",
                column: "VendorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vendors");
        }
    }
}
