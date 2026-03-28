using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBusTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusType",
                table: "Buses");

            migrationBuilder.AddColumn<int>(
                name: "BusTypeId",
                table: "Buses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusTypes",
                columns: table => new
                {
                    BusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultSeats = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusTypes", x => x.BusTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buses_BusTypeId",
                table: "Buses",
                column: "BusTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_BusTypes_BusTypeId",
                table: "Buses",
                column: "BusTypeId",
                principalTable: "BusTypes",
                principalColumn: "BusTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buses_BusTypes_BusTypeId",
                table: "Buses");

            migrationBuilder.DropTable(
                name: "BusTypes");

            migrationBuilder.DropIndex(
                name: "IX_Buses_BusTypeId",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "BusTypeId",
                table: "Buses");

            migrationBuilder.AddColumn<string>(
                name: "BusType",
                table: "Buses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
