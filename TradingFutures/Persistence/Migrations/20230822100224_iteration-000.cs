using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingFutures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class iteration000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettingsItem",
                columns: table => new
                {
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BoolValue = table.Column<bool>(type: "bit", nullable: false),
                    IntValue = table.Column<int>(type: "int", nullable: false),
                    DecimalValue = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsItem", x => new { x.TypeName, x.PropertyName });
                });

            migrationBuilder.CreateTable(
                name: "TradingPositionSettings",
                columns: table => new
                {
                    Asset = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrderSide = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AssistantBuyEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AssistantSellEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TradingBotEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TradingBotCompensationMode = table.Column<bool>(type: "bit", nullable: false),
                    TradingBotForceSell = table.Column<bool>(type: "bit", nullable: false),
                    CreateDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingPositionSettings", x => new { x.Asset, x.OrderSide });
                });

            migrationBuilder.CreateTable(
                name: "TradingProfiles",
                columns: table => new
                {
                    TradingProfileId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingProfiles", x => x.TradingProfileId);
                });

            migrationBuilder.CreateTable(
                name: "TradingTransaction",
                columns: table => new
                {
                    OrderDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Asset = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrderSide = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ContractCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OrderOffset = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    TradeVolume = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    TradeTurnover = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    TradeAvgPrice = table.Column<decimal>(type: "decimal(38,19)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    FeeAsset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    RealProfit = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    LeverageRate = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingTransaction", x => new { x.OrderDT, x.Asset, x.OrderSide });
                });

            migrationBuilder.CreateTable(
                name: "TradingConditions",
                columns: table => new
                {
                    TradingProfileId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConditionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SlopeShortMode = table.Column<short>(type: "smallint", nullable: false),
                    SlopeTrendShortMode = table.Column<short>(type: "smallint", nullable: false),
                    SlopeShortThreshold = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    SlopeLongMode = table.Column<short>(type: "smallint", nullable: false),
                    SlopeTrendLongMode = table.Column<short>(type: "smallint", nullable: false),
                    SlopeLongThreshold = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    CrossDiffMode = table.Column<short>(type: "smallint", nullable: false),
                    CrossDiffThreshold = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    PriceDiffShortMode = table.Column<short>(type: "smallint", nullable: false),
                    PriceDiffShortThreshold = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    PriceDiffLongMode = table.Column<short>(type: "smallint", nullable: false),
                    PriceDiffLongThreshold = table.Column<decimal>(type: "decimal(38,19)", nullable: false),
                    CreateDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingConditions", x => new { x.TradingProfileId, x.ConditionType, x.Interval });
                    table.ForeignKey(
                        name: "FK_TradingConditions_TradingProfiles_TradingProfileId",
                        column: x => x.TradingProfileId,
                        principalTable: "TradingProfiles",
                        principalColumn: "TradingProfileId");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingsItem");

            migrationBuilder.DropTable(
                name: "TradingConditions");

            migrationBuilder.DropTable(
                name: "TradingPositionSettings");

            migrationBuilder.DropTable(
                name: "TradingTransaction");

            migrationBuilder.DropTable(
                name: "TradingProfiles");
        }
    }
}
