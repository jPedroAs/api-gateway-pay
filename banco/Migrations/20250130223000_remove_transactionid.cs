using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banco.Migrations
{
    /// <inheritdoc />
    public partial class removetransactionid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "AccountBaks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "AccountBaks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
