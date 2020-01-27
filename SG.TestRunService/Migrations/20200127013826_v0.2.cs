using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class v02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamProject = table.Column<string>(nullable: false),
                    AzureBuildDefinitionId = table.Column<int>(nullable: false),
                    AzureBuildId = table.Column<int>(nullable: false),
                    SourceVersion = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    BuildNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestCase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AzureTestCaseId = table.Column<int>(nullable: false),
                    TeamProject = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestRunSession",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductBuildInfoId = table.Column<int>(nullable: false),
                    AzureTestBuildId = table.Column<int>(nullable: false),
                    AzureTestBuildNumber = table.Column<string>(nullable: false),
                    SuiteName = table.Column<string>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRunSession_BuildInfo_ProductBuildInfoId",
                        column: x => x.ProductBuildInfoId,
                        principalTable: "BuildInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseImpactCodeSignature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<int>(nullable: false),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: false),
                    Signature = table.Column<string>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateRemoved = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseImpactCodeSignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactCodeSignature_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestLastState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<int>(nullable: false),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: false),
                    ProductBuildInfoId = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    LastOutcome = table.Column<int>(nullable: true),
                    ShouldBeRun = table.Column<bool>(nullable: false),
                    RunReason = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLastState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestLastState_BuildInfo_ProductBuildInfoId",
                        column: x => x.ProductBuildInfoId,
                        principalTable: "BuildInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLastState_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LastImpactUpdate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    ProductBuildInfoId = table.Column<int>(nullable: false),
                    TestRunSessionId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastImpactUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastImpactUpdate_BuildInfo_ProductBuildInfoId",
                        column: x => x.ProductBuildInfoId,
                        principalTable: "BuildInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LastImpactUpdate_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestRun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRunSessionId = table.Column<int>(nullable: false),
                    TestCaseId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    Outcome = table.Column<int>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRun_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestRun_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRunId = table.Column<int>(nullable: true),
                    TestRunSessionId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_TestRun_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachment_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<int>(nullable: true),
                    TestRunId = table.Column<int>(nullable: true),
                    TestRunSessionId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraData_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraData_TestRun_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraData_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TestRunId",
                table: "Attachment",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TestRunSessionId",
                table: "Attachment",
                column: "TestRunSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildInfo_AzureBuildDefinitionId_AzureBuildId",
                table: "BuildInfo",
                columns: new[] { "AzureBuildDefinitionId", "AzureBuildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraData_TestCaseId",
                table: "ExtraData",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraData_TestRunId",
                table: "ExtraData",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraData_TestRunSessionId",
                table: "ExtraData",
                column: "TestRunSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LastImpactUpdate_AzureProductBuildDefinitionId",
                table: "LastImpactUpdate",
                column: "AzureProductBuildDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LastImpactUpdate_ProductBuildInfoId",
                table: "LastImpactUpdate",
                column: "ProductBuildInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LastImpactUpdate_TestRunSessionId",
                table: "LastImpactUpdate",
                column: "TestRunSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCase_AzureTestCaseId",
                table: "TestCase",
                column: "AzureTestCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_Signature",
                table: "TestCaseImpactCodeSignature",
                column: "Signature");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_TestCaseId_AzureProductBuildDefinitionId_Signature",
                table: "TestCaseImpactCodeSignature",
                columns: new[] { "TestCaseId", "AzureProductBuildDefinitionId", "Signature" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_ProductBuildInfoId",
                table: "TestLastState",
                column: "ProductBuildInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_TestCaseId_AzureProductBuildDefinitionId",
                table: "TestLastState",
                columns: new[] { "TestCaseId", "AzureProductBuildDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_TestCaseId",
                table: "TestRun",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_TestRunSessionId_TestCaseId",
                table: "TestRun",
                columns: new[] { "TestRunSessionId", "TestCaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestRunSession_ProductBuildInfoId",
                table: "TestRunSession",
                column: "ProductBuildInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "ExtraData");

            migrationBuilder.DropTable(
                name: "LastImpactUpdate");

            migrationBuilder.DropTable(
                name: "TestCaseImpactCodeSignature");

            migrationBuilder.DropTable(
                name: "TestLastState");

            migrationBuilder.DropTable(
                name: "TestRun");

            migrationBuilder.DropTable(
                name: "TestCase");

            migrationBuilder.DropTable(
                name: "TestRunSession");

            migrationBuilder.DropTable(
                name: "BuildInfo");
        }
    }
}
