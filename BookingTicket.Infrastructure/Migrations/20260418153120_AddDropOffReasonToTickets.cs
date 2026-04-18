using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDropOffReasonToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropOffReason",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropOffReason",
                table: "Tickets");
        }
    }
}
