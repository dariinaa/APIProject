using AutoMapper;
using Companies.Domain.Abstraction.Mappers;
using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services.Auth;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Services.Auth;
using Companies.Domain.Services.Mappers;
using Companies.Domain.Services.Repositories.Auth;
using Companies.Domain.Services.Repositories;
using Companies.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Companies.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDataBaseContext>(provider =>
                new DataBaseContext(configuration.GetConnectionString("SqliteConnection")));

            services.AddScoped<IMyMapper, MyMapper>();

            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IIndustryService, IndustryService>();
            services.AddScoped<IIndustryRepository, IndustryRepository>();

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            services.AddScoped<IReadCsvFileService, ReadCsvFileService>();
            services.AddScoped<IReadCsvFileRepository, ReadCsvFileRepository>()
                ;
            services.AddScoped<ICompanyIndustryAssociation, CompanyIndustryAssociation>();
            services.AddScoped<IPdfSummaryService, PdfSummaryService>();

            services.AddScoped<IDailyStatisticsService, DailyStatisticsService>();
            services.AddScoped<IDailyStatisticsRepository, DailyStatisticsRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<ITokenService, TokenService>();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            services.AddAuthorization();
        }

        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
            });
        }
    }
}
