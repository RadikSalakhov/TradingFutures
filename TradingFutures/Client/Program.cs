using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TradingFutures.Client;
using TradingFutures.Client.Abstraction;
using TradingFutures.Client.Hubs;
using TradingFutures.Client.Services;
using TradingFutures.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Scoped
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IBrowserService, BrowserService>();

builder.Services.AddScoped<ITradingClientService, TradingClientService>();

//Singleton
builder.Services.AddSingleton<IHubClient, HubClient>();

builder.Services.AddSingleton<IClientSettingsService, ClientSettingsService>();

//Regitrations
builder.Services.AddSharedServices(builder.Configuration);

await builder.Build().RunAsync();