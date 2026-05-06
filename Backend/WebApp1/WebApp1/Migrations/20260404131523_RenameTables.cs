using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Installments_ClientBookings_ClientBookingBookingID",
                table: "Installments");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "ClientBookings");

            migrationBuilder.RenameColumn(
                name: "ClientBookingBookingID",
                table: "Installments",
                newName: "UnitBookingBookingID");

            migrationBuilder.RenameIndex(
                name: "IX_Installments_ClientBookingBookingID",
                table: "Installments",
                newName: "IX_Installments_UnitBookingBookingID");

            migrationBuilder.CreateTable(
                name: "ClientExtraDetails",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NationalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalIdImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientExtraDetails", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UnitBooking",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationAmount = table.Column<int>(type: "int", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownPayment = table.Column<int>(type: "int", nullable: true),
                    FirstInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallmentYears = table.Column<int>(type: "int", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    ProjectCode = table.Column<int>(type: "int", nullable: true),
                    UnitID = table.Column<int>(type: "int", nullable: true),
                    Reserved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitBooking", x => x.BookingID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_UnitBooking_UnitBookingBookingID",
                table: "Installments",
                column: "UnitBookingBookingID",
                principalTable: "UnitBooking",
                principalColumn: "BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Installments_UnitBooking_UnitBookingBookingID",
                table: "Installments");

            migrationBuilder.DropTable(
                name: "ClientExtraDetails");

            migrationBuilder.DropTable(
                name: "UnitBooking");

            migrationBuilder.RenameColumn(
                name: "UnitBookingBookingID",
                table: "Installments",
                newName: "ClientBookingBookingID");

            migrationBuilder.RenameIndex(
                name: "IX_Installments_UnitBookingBookingID",
                table: "Installments",
                newName: "IX_Installments_ClientBookingBookingID");

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalIdImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ClientBookings",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    DownPayment = table.Column<int>(type: "int", nullable: true),
                    FirstInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallmentYears = table.Column<int>(type: "int", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectCode = table.Column<int>(type: "int", nullable: true),
                    ReservationAmount = table.Column<int>(type: "int", nullable: true),
                    Reserved = table.Column<bool>(type: "bit", nullable: true),
                    UnitID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientBookings", x => x.BookingID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_ClientBookings_ClientBookingBookingID",
                table: "Installments",
                column: "ClientBookingBookingID",
                principalTable: "ClientBookings",
                principalColumn: "BookingID");
        }
    }
}
