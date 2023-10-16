using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingFutures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class iteration001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TradingProfileId",
                table: "TradingPositionSettings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingProfileId",
                table: "TradingPositionSettings");
        }
    }
}
