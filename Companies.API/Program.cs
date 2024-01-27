using AutoMapper;
using Companies.API.Middleware;
using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services;
using Companies.Domain.Services.Mappers;
using Companies.Domain.Services.Repositories;
using Serilog;

SQLitePCL.Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//sqlite 
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");

//DataBaseContext
builder.Services.AddSingleton<IDataBaseContext>(provider => new DataBaseContext(connectionString));

builder.Services.AddScoped<IMyMapper, MyMapper>();

//using automapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//services
builder.Services.AddScoped<IIndustryService, IndustryService>();
builder.Services.AddScoped<IIndustryRepository, IndustryRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IReadCsvFileService, ReadCsvFileService>();
builder.Services.AddScoped<IReadCsvFileRepository, ReadCsvFileRepository>();

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

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IDataBaseContext>();
    await dbContext.ResetDatabase();
    //await dbContext.InitializeDatabase();
}

app.Run();
