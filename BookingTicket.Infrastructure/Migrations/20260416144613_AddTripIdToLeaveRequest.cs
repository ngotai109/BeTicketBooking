using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripIdToLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TripId",
                table: "DriverLeaveRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverLeaveRequests_TripId",
                table: "DriverLeaveRequests",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverLeaveRequests_Trips_TripId",
                table: "DriverLeaveRequests",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverLeaveRequests_Trips_TripId",
                table: "DriverLeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_DriverLeaveRequests_TripId",
                table: "DriverLeaveRequests");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "DriverLeaveRequests");
        }
    }
}
