using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamBuddy.API.Data;
using StreamBuddy.API.Models;
using StreamBuddy.API.GraphQL;
using StreamBuddy.API.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// Load config
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ Connection string is missing.");
    throw new InvalidOperationException("Database connection string is not set.");
}

// Then register
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
    
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Set Up GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddMutationType<Mutation>();

// In Program.cs (before builder.Build()):
builder.Configuration.AddEnvironmentVariables();

var rapidApiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
var rapidApiHost = Environment.GetEnvironmentVariable("RAPIDAPI_HOST");
var rapidApiBaseUrl = Environment.GetEnvironmentVariable("RAPIDAPI_BASE_URL");

var app = builder.Build();

// Middleware
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

app.MapGraphQL("/graphql");

app.Run();