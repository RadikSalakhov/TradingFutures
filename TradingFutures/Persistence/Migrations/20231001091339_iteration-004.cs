using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingFutures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class iteration004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingBotStrategyType",
                table: "TradingPositionSettings");

            migrationBuilder.DropColumn(
                name: "TradingProfileId",
                table: "TradingPositionSettings");

            migrationBuilder.AddColumn<bool>(
                name: "TradingBotSellMode",
                table: "TradingPositionSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingBotSellMode",
                table: "TradingPositionSettings");

            migrationBuilder.AddColumn<string>(
                name: "TradingBotStrategyType",
                table: "TradingPositionSettings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TradingProfileId",
                table: "TradingPositionSettings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
