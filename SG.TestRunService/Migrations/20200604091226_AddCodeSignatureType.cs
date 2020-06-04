using Microsoft.EntityFrameworkCore.Migrations;
using SG.TestRunService.Common.Models;

namespace SG.TestRunService.Migrations
{
    public partial class AddCodeSignatureType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "CodeSignature",
                nullable: false,
                defaultValue: CodeSignatureType.File);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "CodeSignature");
        }
    }
}
