using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueKeyServer.Migrations
{
    public partial class eightMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "taskNumber",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "taskNumber",
                table: "Messages");
        }
    }
}
