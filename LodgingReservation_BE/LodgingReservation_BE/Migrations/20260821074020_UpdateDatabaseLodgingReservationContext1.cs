using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LodgingReservation_BE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseLodgingReservationContext1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IS_DELETED",
                table: "USER",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PHONE_NUMBER",
                table: "USER",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMAGE_URL",
                table: "ROOM",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_DELETED",
                table: "USER");

            migrationBuilder.DropColumn(
                name: "PHONE_NUMBER",
                table: "USER");

            migrationBuilder.DropColumn(
                name: "IMAGE_URL",
                table: "ROOM");
        }
    }
}
