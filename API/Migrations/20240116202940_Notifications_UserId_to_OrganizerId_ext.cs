using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Notifications_UserId_to_OrganizerId_ext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Notifications_User",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "userID",
                table: "Notifications",
                newName: "organizerID");

            migrationBuilder.RenameIndex(
                name: "Notifications_User",
                table: "Notifications",
                newName: "Notifications_Organizer");

            migrationBuilder.AddForeignKey(
                name: "Notifications_Organizer",
                table: "Notifications",
                column: "organizerID",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Notifications_Organizer",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "organizerId",
                table: "Notifications",
                newName: "userID");

            migrationBuilder.RenameIndex(
                name: "Notifications_Organizer",
                table: "Notifications",
                newName: "Notifications_User");

            migrationBuilder.AddForeignKey(
                name: "Notifications_User",
                table: "Notifications",
                column: "userID",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
