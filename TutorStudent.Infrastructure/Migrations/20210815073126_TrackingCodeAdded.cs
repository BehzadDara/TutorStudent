using Microsoft.EntityFrameworkCore.Migrations;

namespace TutorStudent.Infrastructure.Migrations
{
    public partial class TrackingCodeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingCode",
                table: "Applies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingCode",
                table: "Applies");
        }
    }
}
