using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPassengerNoteToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PassengerNote",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerNote",
                table: "Tickets");
        }
    }
}
