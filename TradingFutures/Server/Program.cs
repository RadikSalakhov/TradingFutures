using Microsoft.EntityFrameworkCore;
using Serilog;
using TradingFutures.Application;
using TradingFutures.Infrastructure;
using TradingFutures.Persistence;
using TradingFutures.Persistence.Contexts;
using TradingFutures.Server.Hubs;
using TradingFutures.Server.WorkerHandlers;
using TradingFutures.Server.Workers;
using TradingFutures.Shared;

var corsPolicyName = "signalrhub-cors-policy";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();

builder.Configuration.AddJsonFile("appsettings-contracts.json");

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

builder.Services.AddSharedServices(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddSingleton<IWorkerHandler, WorkerHandler>();

builder.Services.AddHostedService<EmaCrossIndicatorWorker>();
builder.Services.AddHostedService<HuobiWorker>();
builder.Services.AddHostedService<NotificationsHubWorker>();
builder.Services.AddHostedService<TradingWorker>();

//builder.WebHost.UseUrls("https://localhost:6001", "http://localhost:6000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(corsPolicyName);

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapHub<NotificationsHub>("/hub");

//Apply last migrations
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

app.Run();