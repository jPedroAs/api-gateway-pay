using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banco.Migrations
{
    /// <inheritdoc />
    public partial class addedaccounttransf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Account_transferred",
                table: "Transactions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account_transferred",
                table: "Transactions");
        }
    }
}
