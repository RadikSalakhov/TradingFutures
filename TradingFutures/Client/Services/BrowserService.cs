using Microsoft.JSInterop;
using System.Drawing;
using TradingFutures.Client.Abstraction;

namespace TradingFutures.Client.Services
{
    public class BrowserService : IBrowserService
    {
        private readonly IJSRuntime _js;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitializeResizeListener(object refObj)
        {
            await _js.InvokeAsync<string>("resizeListener", DotNetObjectReference.Create(refObj));
        }

        public async Task<Size> GetWindowSize()
        {
            return await _js.InvokeAsync<Size>("getWindowSize");
        }
    }
}