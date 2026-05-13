using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class setInstallmentsViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW  vw_ClientFullDetails AS
                                    SELECT c.*,
                                    e.PhoneNumber
                                    FROM ClientExtraDetails c 
                                    INNER JOIN Clients e 
                                    ON c.ClientID=e.ClientID");
            migrationBuilder.Sql(@"CREATE VIEW vvw_ClientUnitsBookings AS
                                    SELECT u.*, 
                                    i.InstallmentID,
                                    i.InstallmentNumber,
                                    i.DueDate,  
                                    i.MonthlyAmount,
                                    i.Paid,
                                    i.PaymentType,
                                    i.CheckImage   
                                    FROM  UnitBooking u  
                                    INNER JOIN Installments i
                                    ON u.BookingID = i.BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vw_ClientFullDetails");
            migrationBuilder.Sql(@"DROP VIEW vvw_ClientUnitsBookings");
        }
    }
}
