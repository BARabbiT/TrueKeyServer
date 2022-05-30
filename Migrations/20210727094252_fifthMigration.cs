using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueKeyServer.Migrations
{
    public partial class fifthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GetEmailMessage",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GetSubMessage",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FRData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    Inn = table.Column<string>(type: "text", nullable: true),
                    AdressPlaceOfS = table.Column<string>(type: "text", nullable: true),
                    NDS = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    FirmwareID = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    RegisterNumber = table.Column<string>(type: "text", nullable: true),
                    NomberFN = table.Column<string>(type: "text", nullable: true),
                    VersConfigur = table.Column<string>(type: "text", nullable: true),
                    VersBoot = table.Column<string>(type: "text", nullable: true),
                    VersionFn = table.Column<string>(type: "text", nullable: true),
                    OperatorFD = table.Column<string>(type: "text", nullable: true),
                    RegFnCount = table.Column<string>(type: "text", nullable: true),
                    AmountFnRereg = table.Column<string>(type: "text", nullable: true),
                    CheckResource = table.Column<string>(type: "text", nullable: true),
                    FFD = table.Column<string>(type: "text", nullable: true),
                    EndDataFN = table.Column<string>(type: "text", nullable: true),
                    FnExpireDays = table.Column<string>(type: "text", nullable: true),
                    QueueDocOFD = table.Column<string>(type: "text", nullable: true),
                    NumFirstUnDoc = table.Column<string>(type: "text", nullable: true),
                    DateFirstUnDoc = table.Column<string>(type: "text", nullable: true),
                    StateInfoEx = table.Column<string>(type: "text", nullable: true),
                    LastModifiedDate = table.Column<string>(type: "text", nullable: true),
                    LastRegDateFN = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FRData");

            migrationBuilder.DropColumn(
                name: "GetEmailMessage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GetSubMessage",
                table: "Users");
        }
    }
}
