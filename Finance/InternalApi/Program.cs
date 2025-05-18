
using Cryptograf;
using DataModel.DataBase;
using InternalApi.EntityGateWay;
using InternalApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InternalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddRateLimiter(_ =>
            {
                _.AddFixedWindowLimiter("default", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromSeconds(1);
                    options.QueueLimit = 0;
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
            });

            builder.Services.AddSingleton<BasicConfiguration>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new BasicConfiguration(configuration);
            });

            var configuration = builder?.Services?.BuildServiceProvider().GetRequiredService<BasicConfiguration>();
            _ = builder?.Services.AddDbContext<FinanceDBContext>(options => options.UseNpgsql(configuration?.ConnectionString));

            ServicesBinding(builder!.Services);
            AuthenticationBinding(builder!.Services, configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".svg"] = "image/svg+xml" }
                }
            });

            app.UseRateLimiter();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static void ServicesBinding(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IRepository<PlotDTO, long>, PlotRepository>();
            serviceCollection.AddScoped<IRepository<TableDTO, long>, TableRepository>();
            serviceCollection.AddScoped<IRepository<MainBoardDTO, long>, MainBoardRepository>();

            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<ITableService, TableService>();
            serviceCollection.AddScoped<IPlotService, PlotService>();

            serviceCollection.AddScoped<FinanceService>();
            serviceCollection.AddScoped<ReportService>();

            serviceCollection.AddSingleton<ICryptoService, CryptoService>();
        }

        private static void AuthenticationBinding(IServiceCollection serviceCollection, BasicConfiguration configuration)
        {
            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.SecretJWT)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration.IssuerJWT,
                    ValidateAudience = true,
                    ValidAudience = configuration.AudienceJWT,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
