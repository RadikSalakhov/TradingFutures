using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TradingFutures.Application.Configuration;

namespace TradingFutures.Server.Controllers.Base
{
    public abstract class BaseWebApiController : ControllerBase
    {
        private const string TEMP_STR = "JTB4udt-qrt*zwx6rkm";

        private readonly bool _skipPasswordCheck;

        protected BaseWebApiController(IOptions<DevOptions> devOptions)
        {
            _skipPasswordCheck = devOptions.Value.ApiPassword == TEMP_STR;
        }

        protected bool ValidatePassword(string? pwd)
        {
            return _skipPasswordCheck || pwd == TEMP_STR;
        }
    }
}