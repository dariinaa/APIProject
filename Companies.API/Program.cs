using Companies.API.Extensions;
using Companies.API.Middleware;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;
using Companies.Domain.Services.Scheduler;
using Companies.Domain.Settings;
using FluentScheduler;
using Serilog;
using System.Configuration;

SQLitePCL.Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

//load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

builder.Services.Configure<PdfSettings>(configuration.GetSection("PdfSettings"));
builder.Services.Configure<JSONSettings>(configuration.GetSection("JSONSettings"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

//ip address safe list 
app.UseMiddleware<SafelistMiddleware>(builder.Configuration["AdminSafeList"]);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//auth
app.UseAuthentication();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IDataBaseContext>();
    //await dbContext.ResetDatabase();
    await dbContext.InitializeDatabase();

    var dailyStatisticsService = scope.ServiceProvider.GetRequiredService<IDailyStatisticsService>();
    JobManager.Initialize(new MyRegistry(dailyStatisticsService));
}

app.Run();
