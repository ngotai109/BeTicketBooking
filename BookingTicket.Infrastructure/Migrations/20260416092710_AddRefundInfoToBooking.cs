using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundInfoToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefundAccountName",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundAccountNumber",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundBankName",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundAccountName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RefundAccountNumber",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RefundBankName",
                table: "Bookings");
        }
    }
}
