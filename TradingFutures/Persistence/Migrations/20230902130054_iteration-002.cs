using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingFutures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class iteration002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingBotCompensationMode",
                table: "TradingPositionSettings");

            migrationBuilder.DropColumn(
                name: "TradingBotForceSell",
                table: "TradingPositionSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TradingBotCompensationMode",
                table: "TradingPositionSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TradingBotForceSell",
                table: "TradingPositionSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
