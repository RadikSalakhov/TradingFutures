using System.Drawing;

namespace TradingFutures.Client.Abstraction
{
    public interface IBrowserService
    {
        Task InitializeResizeListener(object refObj);

        Task<Size> GetWindowSize();
    }
}