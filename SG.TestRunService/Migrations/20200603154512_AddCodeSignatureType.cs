using Microsoft.EntityFrameworkCore.Migrations;
using SG.TestRunService.Data;

namespace SG.TestRunService.Migrations
{
    public partial class AddCodeSignatureType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
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
