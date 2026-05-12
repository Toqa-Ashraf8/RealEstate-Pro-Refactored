using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApp1.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class setReservedClientsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW vw_Booked_Clients AS 
            SELECT n.*,
            u.BookingID,
            u.BookingDate
            FROM Negotiations n 
            INNER JOIN UnitBooking u 
            ON n.UnitID=u.UnitID 
            AND n.ProjectCode=u.ProjectCode
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vw_Booked_Clients");
        }
    }
}
