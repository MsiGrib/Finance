
using InternalApi.EntityGateWay;
using InternalApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static void ServicesBinding(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IUserService, UserService>();
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
