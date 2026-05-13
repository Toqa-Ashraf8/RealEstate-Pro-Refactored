using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Migrations
{
    /// <inheritdoc />
    public partial class setView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ClientFullDetails");

            migrationBuilder.Sql(@"CREATE VIEW vw_ClientFullDetails AS
                        SELECT c.*, p.PhoneNumber
                        FROM ClientExtraDetails c
                        INNER JOIN Clients p ON c.ClientID = p.ClientID");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ClientFullDetails");

            migrationBuilder.Sql(@"CREATE VIEW vw_ClientFullDetails AS
                        SELECT c.*, e.PhoneNumber
                        FROM ClientExtraDetails c 
                        INNER JOIN Clients e ON c.ClientID = e.ClientID");

        }
    }
}
