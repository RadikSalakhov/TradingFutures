using System.Drawing;
using TradingFutures.Client.Services.ClientSettingsEntries.Base;

namespace TradingFutures.Client.Services.ClientSettingsEntries
{
    public class WindowSizeClientSettingsEntry : BaseClientSettingsEntry<Size>
    {
        private const decimal REF_ASSET_ITEM_WIDTH = 320m;//350m
        private const decimal REF_ASSET_ITEM_HEIGHT = 180m;//175m

        public bool IsDesktop => Value.Width >= 1200;

        public int ClientAreaHeight
        {
            get
            {
                if (IsDesktop)
                    return Value.Height - 100 + 2;
                else
                    return Value.Height - 240 + 2;
            }
        }

        public int GetVisibleColumnsAmount()
        {
            var columns = (int)Math.Floor(Value.Width / REF_ASSET_ITEM_WIDTH);
            return columns > 0 ? columns : 1;
        }

        public int GetVisibleRowsAmount()
        {
            var rows = (int)Math.Floor(ClientAreaHeight / REF_ASSET_ITEM_HEIGHT);
            return rows > 0 ? rows : 1;
        }

        public int GetRowHeight(int rowsAmount)
        {
            const int ROW_GAP = 10;
            const int PADDING = 10;

            return (ClientAreaHeight - (rowsAmount - 1) * ROW_GAP - 2 * PADDING) / rowsAmount;
        }
    }
}