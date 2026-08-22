using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LodgingReservation.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToRoomType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IMAGE_URL",
                table: "ROOM_TYPE",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMAGE_URL",
                table: "ROOM_TYPE");
        }
    }
}
