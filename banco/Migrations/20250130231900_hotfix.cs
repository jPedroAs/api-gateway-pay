using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banco.Migrations
{
    /// <inheritdoc />
    public partial class hotfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AccountBaks_AccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Account_transferred",
                table: "Transactions",
                column: "Account_transferred");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AccountBaks_Account_transferred",
                table: "Transactions",
                column: "Account_transferred",
                principalTable: "AccountBaks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AccountBaks_Account_transferred",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Account_transferred",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AccountBaks_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "AccountBaks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
