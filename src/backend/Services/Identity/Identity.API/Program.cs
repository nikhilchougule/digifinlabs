using Genie.Persistence;
using Identity.Application;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //create webapplication createbuilder object with args
            var builder = WebApplication.CreateBuilder(args);

            // ── Persistence ───────────────────────────────────────────────────
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection connection string is required.");
            builder.Services.AddGeniePersistence(connectionString);

            // ── Application + Infrastructure ──────────────────────────────────
            builder.Services.AddIdentityApplication();
            builder.Services.AddIdentityInfrastructure();

            // ── JWT Authentication ────────────────────────────────────────────
            var jwtSecret = builder.Configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is required.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            builder.Services.AddAuthorization();

            // ── CORS ──────────────────────────────────────────────────────────
            builder.Services.AddCors(opt =>
                opt.AddDefaultPolicy(p =>
                    p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
                     .AllowAnyHeader()
                     .AllowAnyMethod()));

            // ── OpenAPI ───────────────────────────────────────────────────────
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
