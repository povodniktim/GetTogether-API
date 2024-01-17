using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class EventParticipants_UpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "EventParticipants",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "statusChangedAt",
                table: "EventParticipants",
                newName: "createdAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedAt",
                table: "EventParticipants",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "EventParticipants",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "EventParticipants",
                newName: "statusChangedAt");

            migrationBuilder.AlterColumn<int>(
                name: "participantID",
                table: "Notifications",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "eventID",
                table: "Notifications",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "EventParticipants",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "Notifications_Event",
                table: "Notifications",
                column: "eventID",
                principalTable: "Events",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Notifications_Participant",
                table: "Notifications",
                column: "participantID",
                principalTable: "EventParticipants",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
