using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class intitDBnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReminderSent",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReminderSent",
                table: "Bookings");
        }
    }
}
