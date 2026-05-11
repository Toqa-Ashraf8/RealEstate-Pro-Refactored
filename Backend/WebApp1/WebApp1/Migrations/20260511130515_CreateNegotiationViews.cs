using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class CreateNegotiationViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW Negotiations_2  AS
            SELECT n.*,  
                r.NegotiationCondition, 
                r.SuggestedPrice, 
                r.ReasonOfReject, 
                r.CheckedDate
                FROM Negotiations n 
                INNER JOIN Rejected_negotiations_phases r 
                ON n.ClientID = r.ClientID 
                AND n.ProjectCode = r.ProjectCode 
                AND n.UnitID = r.UnitID
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW Negotiations_2");
        }
    }
}
