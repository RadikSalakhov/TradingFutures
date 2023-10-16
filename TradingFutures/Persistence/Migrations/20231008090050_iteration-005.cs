using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingFutures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class iteration005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TradingBotSellMode",
                table: "TradingPositionSettings",
                newName: "EmaCrossLogicOpen");

            migrationBuilder.AddColumn<bool>(
                name: "EmaCrossLogicClose",
                table: "TradingPositionSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmaCrossLogicClose",
                table: "TradingPositionSettings");

            migrationBuilder.RenameColumn(
                name: "EmaCrossLogicOpen",
                table: "TradingPositionSettings",
                newName: "TradingBotSellMode");
        }
    }
}
