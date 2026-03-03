using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorStores.Infrastructure.Migrations
{
    public partial class addroleandcreatedby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "UserIds",
                newName: "createdBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserIds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserIds",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserIds");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserIds");

            migrationBuilder.RenameColumn(
                name: "createdBy",
                table: "UserIds",
                newName: "CreatedBy");
        }
    }
}
