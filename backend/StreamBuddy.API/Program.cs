using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamBuddy.API.Data;
using StreamBuddy.API.Services;
using DotNetEnv;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory; // ✅ Import DotNetEnv

var builder = WebApplication.CreateBuilder(args);

Env.Load();

Console.WriteLine($"🔍 RAPIDAPI_KEY: {Environment.GetEnvironmentVariable("RAPIDAPI_KEY") ?? "❌ NOT FOUND"}");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ Connection string is missing. Make sure appsettings.json or .env is configured properly.");
    throw new InvalidOperationException("Database connection string is not set.");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "DefaultSecretKey");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddSingleton<StreamingService>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGraphQL();
app.MapControllers();

app.Run();
