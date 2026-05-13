using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class updateView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vvw_ClientUnitsBookings");
            migrationBuilder.Sql(@"CREATE VIEW vw_ClientUnitsBookings AS
                        SELECT 
                        u.BookingID, 
                        u.ReservationAmount,
                        u.PaymentMethod, 
                        u.CheckImagePath, 
                        u.DownPayment, 
                        u.FirstInstallmentDate, 
                        u.InstallmentYears, 
                        u.BookingDate, 
                        u.ClientID, 
                        u.ProjectCode, 
                        u.UnitID, 
                        u.Reserved, 
                        i.InstallmentID, 
                        i.InstallmentNumber, 
                        i.DueDate, 
                        i.MonthlyAmount, 
                        i.Paid, 
                        i.PaymentType, 
                        i.CheckImage, n.ClientName, n.ProjectName, n.unitName
                        FROM     UnitBooking AS u INNER JOIN
                  Installments AS i ON u.BookingID = i.BookingID INNER JOIN
                  Negotiations n ON u.ClientID =n.ClientID AND u.ProjectCode = n.ProjectCode AND u.UnitID = n.UnitID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vvw_ClientUnitsBookings");
            migrationBuilder.Sql(@"CREATE VIEW vw_ClientUnitsBookings AS
                        SELECT 
                        u.BookingID, 
                        u.ReservationAmount,
                        u.PaymentMethod, 
                        u.CheckImagePath, 
                        u.DownPayment, 
                        u.FirstInstallmentDate, 
                        u.InstallmentYears, 
                        u.BookingDate, 
                        u.ClientID, 
                        u.ProjectCode, 
                        u.UnitID, 
                        u.Reserved, 
                        i.InstallmentID, 
                        i.InstallmentNumber, 
                        i.DueDate, 
                        i.MonthlyAmount, 
                        i.Paid, 
                        i.PaymentType, 
                        i.CheckImage, n.ClientName, n.ProjectName, n.unitName
                        FROM     UnitBooking AS u INNER JOIN
                  Installments AS i ON u.BookingID = i.BookingID INNER JOIN
                  Negotiations n ON u.ClientID =n.ClientID AND u.ProjectCode = n.ProjectCode AND u.UnitID = n.UnitID");

        }
    }
}
