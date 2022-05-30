using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueKeyServer.Migrations
{
    public partial class sevenMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageCreator",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageCreator",
                table: "Messages");
        }
    }
}
