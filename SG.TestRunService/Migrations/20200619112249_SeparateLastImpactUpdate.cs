using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class SeparateLastImpactUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLastState_BuildInfo_ProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.DropIndex(
                name: "IX_TestLastState_ProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.RenameColumn(
                name: "ProductBuildInfoId",
                table: "TestLastState",
                newName: "LastOutcomeProductBuildInfoId");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "TestLastState",
                newName: "LastOutcomeDate");

            migrationBuilder.AlterColumn<int>(
                name: "LastOutcome",
                table: "TestLastState",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastImpactedDate",
                table: "TestLastState",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastImpactedProductBuildInfoId",
                table: "TestLastState",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_LastImpactedProductBuildInfoId",
                table: "TestLastState",
                column: "LastImpactedProductBuildInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_LastOutcomeProductBuildInfoId",
                table: "TestLastState",
                column: "LastOutcomeProductBuildInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLastState_BuildInfo_LastImpactedProductBuildInfoId",
                table: "TestLastState",
                column: "LastImpactedProductBuildInfoId",
                principalTable: "BuildInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestLastState_BuildInfo_LastOutcomeProductBuildInfoId",
                table: "TestLastState",
                column: "LastOutcomeProductBuildInfoId",
                principalTable: "BuildInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLastState_BuildInfo_LastImpactedProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.DropForeignKey(
                name: "FK_TestLastState_BuildInfo_LastOutcomeProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.DropIndex(
                name: "IX_TestLastState_LastImpactedProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.DropIndex(
                name: "IX_TestLastState_LastOutcomeProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.DropColumn(
                name: "LastImpactedDate",
                table: "TestLastState");

            migrationBuilder.DropColumn(
                name: "LastImpactedProductBuildInfoId",
                table: "TestLastState");

            migrationBuilder.RenameColumn(
                name: "LastOutcomeDate",
                table: "TestLastState",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "LastOutcomeProductBuildInfoId",
                table: "TestLastState",
                newName: "ProductBuildInfoId");

            migrationBuilder.AlterColumn<int>(
                name: "LastOutcome",
                table: "TestLastState",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_ProductBuildInfoId",
                table: "TestLastState",
                column: "ProductBuildInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLastState_BuildInfo_ProductBuildInfoId",
                table: "TestLastState",
                column: "ProductBuildInfoId",
                principalTable: "BuildInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
