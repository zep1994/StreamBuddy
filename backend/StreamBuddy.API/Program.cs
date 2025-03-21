using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamBuddy.API.Data;
using StreamBuddy.API.Services;
using StreamBuddy.API.GraphQL; // Explicitly import your Query namespace
using HotChocolate.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register Services
builder.Services.AddControllers();
builder.Services.AddSingleton<StreamingService>();

// Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ Connection string is missing.");
    throw new InvalidOperationException("Database connection string is not set.");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Authentication
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<Movie>();

var app = builder.Build();

// Middleware
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();

app.Run();