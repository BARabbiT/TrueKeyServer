using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueKeyServer.Migrations
{
    public partial class sixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    timeChange = table.Column<string>(type: "text", nullable: true),
                    usersUuids = table.Column<string>(type: "text", nullable: true),
                    dateCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    linkToObj = table.Column<string>(type: "text", nullable: true),
                    msg = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    imageSource = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
