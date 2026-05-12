using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class createClientExtraDetailsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW vw_ClientExtraDetails AS
                                SELECT c.*,
                                u.BookingID,
                                u.UnitID,
                                u.ProjectCode
                                FROM ClientExtraDetails c 
                                INNER JOIN UnitBooking u 
                                ON c.ClientID=u.ClientID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop view vw_ClientExtraDetails");
        }
    }
}
