using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamBuddy.API.Data;
using StreamBuddy.API.Services;
using DotNetEnv;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration (Ensures `appsettings.json` is used)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ✅ Register Services
builder.Services.AddControllers();
builder.Services.AddSingleton<StreamingService>();

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();


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


// ✅ Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()  
            .AllowAnyMethod()  
            .AllowAnyHeader()); 
});
 
builder.Services.AddControllers();
builder.Services.AddSingleton<StreamingService>();
builder.Services.AddScoped<UserReactionsService>();

// Register PostgreSQL & GraphQL
builder.Services
    .AddDbContext<AppDbContext>() 
    .AddGraphQLServer()
    .AddQueryType<Query>(); 

var app = builder.Build();

// ✅ Debugging: Print API Key to Verify Configuration is Loaded
var apiKey = builder.Configuration["RapidAPI:ApiKey"];
Console.WriteLine($"✅ PROGRAM.CS: RAPIDAPI_KEY LOADED: {apiKey?.Substring(0, 5)}*****");

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("❌ RAPIDAPI_KEY NOT FOUND. Check appsettings.json or environment variables.");
}
else
{
    Console.WriteLine($"✅ RAPIDAPI_KEY LOADED: {apiKey.Substring(0, 5)}*****"); 
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAllOrigins");


app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();

app.Run();
