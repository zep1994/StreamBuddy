# StreamBuddy

StreamBuddy is a web application designed to help users track and manage their favorite movies and streaming platforms. It consists of a .NET Web API backend and a Next.js frontend.

## Features

-   **.NET Web API Backend:**
    -   Utilizes PostgreSQL for data storage.
    -   Implements GraphQL for efficient data querying.
    -   Includes JWT authentication for secure access.
-   **Next.js Frontend:**
    -   Uses Apollo Client to interact with the GraphQL API.
    -   Displays a list of movies and their streaming platforms.

## Prerequisites

-   .NET 6 or later installed
-   Node.js and npm installed
-   PostgreSQL database setup and running

## Getting Started

### 1. Clone the Repository:

```bash
git clone https://github.com/zep1994/StreamBuddy.git
cd StreamBuddy
```

### 2. Setup the Backend

```bash
cd backend/StreamBuddy.API
```

- Update the database connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=streambuddy;Username=postgres;Password=yourpassword"
}
```

- Apply database migrations:

```bash
dotnet ef database update
```

- Run the API:

```bash
dotnet run
```

The GraphQL endpoint will be available at `http://localhost:5057/graphql`.

### 3. Setup the Frontend

```bash
cd ../../frontend/streambuddy-client
```

- Install dependencies:

```bash
npm install
```

- Start the frontend:

```bash
npm run dev
```

The application will be accessible at `http://localhost:3000`.

## Project Structure

```
StreamBuddy/
├── backend/
│   ├── StreamBuddy.API/
│   └── ...
├── frontend/
│   ├── streambuddy-client/
│   └── ...
└── README.md
```

## Configuration

-   **Database:**
    -   PostgreSQL connection settings are in `backend/StreamBuddy.API/appsettings.json`.
-   **JWT:**
    -   The secret key for JWT authentication is in `backend/StreamBuddy.API/appsettings.json`. Change this to a secure value.
-   **GraphQL Endpoint:**
    -   The Next.js frontend connects to the API at `http://localhost:5057/graphql`, configured in `frontend/streambuddy-client/pages/_app.js`.

## Dependencies

-   **.NET Web API:**
    -   Npgsql.EntityFrameworkCore.PostgreSQL
    -   Microsoft.EntityFrameworkCore.Design
    -   Microsoft.AspNetCore.Authentication.JwtBearer
    -   GraphQL.Server.Transports.AspNetCore
    -   GraphQL.Server.Ui.Playground
    -   GraphQL
    -   GraphQL.MicrosoftDI
    -   HotChocolate.AspNetCore
    -   HotChocolate.Data.EntityFramework
-   **Next.js Frontend:**
    -   @apollo/client
    -   graphql
    -   axios
    -   jsonwebtoken

## Future Improvements

-   Add user authentication to the frontend.
-   Implement better error handling and logging.
-   Add unit and integration tests.
-   Expand GraphQL schema to include more movie details.
-   Improve UI/UX of the frontend.
-   Implement CI/CD pipelines for automated deployment.

## .NET Web API Backend Setup

1. **Create the .NET Web API Project:**

```bash
mkdir backend && cd backend
dotnet new webapi -n StreamBuddy.API
cd StreamBuddy.API
```

2. **Add Required Packages:**

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package GraphQL.Server.Transports.AspNetCore
dotnet add package GraphQL.Server.Ui.Playground
dotnet add package GraphQL
dotnet add package GraphQL.MicrosoftDI
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.Data.EntityFramework
```

3. **Configure the API (appsettings.json):**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=streambuddy;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Secret": "YourVeryLongSecretKeyForJWT"
  }
}
```

4. **Create the Database Context (Data/AppDbContext.cs):**

```csharp
using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
```

5. **Create a Simple Model (Models/Movie.cs):**

```csharp
namespace StreamBuddy.API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
    }
}
```

6. **Setup Dependency Injection in Program.cs:**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamBuddy.API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]);
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

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

app.Run();
```

7. **Create GraphQL Query (Queries/Query.cs):**

```csharp
using StreamBuddy.API.Data;
using StreamBuddy.API.Models;

public class Query
{
    public IQueryable<Movie> GetMovies([Service] AppDbContext context) => context.Movies;
}
```

---

This version removes all Docker references, improves clarity, and refines setup instructions for a non-Dockerized environment.

