# StreamBuddy

StreamBuddy is a web application designed to help users track and manage their favorite movies and streaming platforms. It consists of a .NET Web API backend and a Next.js frontend, all containerized with Docker for easy deployment.

## Features

-   **.NET Web API Backend:**
    -      Utilizes PostgreSQL for data storage.
    -      Implements GraphQL for efficient data querying.
    -      Includes JWT authentication for secure access.
    -      Containerized with Docker for streamlined deployment.
-   **Next.js Frontend:**
    -      Uses Apollo Client to interact with the GraphQL API.
    -      Displays a list of movies and their streaming platforms.
    -      Containerized with Docker for easy deployment.
-   **Docker Compose:**
    -      Orchestrates the backend, database, and frontend services.
    -      Simplifies the setup and deployment process.

## Prerequisites

-   Docker and Docker Compose installed on your machine.

## Getting Started

1.  **Clone the Repository:**

    ```bash
    git clone [https://github.com/zep1994/StreamBuddy.git](https://github.com/zep1994/StreamBuddy.git)
    cd StreamBuddy
    ```

2.  **Build and Run the Docker Containers:**

    ```bash
    docker-compose up --build
    ```

    This command will:

    -      Build the .NET Web API backend image.
    -      Start a PostgreSQL database container.
    -      Build the Next.js frontend image.
    -      Run all services as defined in `docker-compose.yml`.

3.  **Access the Application:**

    -      Open your web browser and navigate to `http://localhost:3000` to view the Next.js frontend.
    -   The Graphql endpoint can be accessed at `http://localhost:5057/graphql`.

## Project Structure
StreamBuddy/

├── backend/

├── frontend/

├── docker-compose.yml     

└── README.md                 

-   `backend/StreamBuddy.API/`: Contains the .NET Web API project.
-   `frontend/streambuddy-client/`: Contains the Next.js frontend project.
-   `docker-compose.yml`: Defines the Docker services and their configurations.

## Configuration

-   **Database:**
    -      The PostgreSQL database connection string is defined in `backend/StreamBuddy.API/appsettings.json`.
    -   You can modify the postgres password in the docker-compose.yml and the appsettings.json file.
-   **JWT:**
    -   The JWT secret is also defined in the `backend/StreamBuddy.API/appsettings.json` file. Change this to a secure secret.
-   **GraphQL Endpoint:**
    -   The Next.js frontend connects to the GraphQL endpoint at `http://localhost:5057/graphql`, which is configured in `frontend/streambuddy-client/pages/_app.js`.

## Dependencies

-   **.NET Web API:**
    -      Npgsql.EntityFrameworkCore.PostgreSQL
    -      Microsoft.EntityFrameworkCore.Design
    -      Microsoft.AspNetCore.Authentication.JwtBearer
    -   GraphQL.Server.Transports.AspNetCore
    -   GraphQL.Server.Ui.Playground
    -   GraphQL
    -   GraphQL.MicrosoftDI
    -   HotChocolate.AspNetCore
    -   HotChocolate.Data.EntityFramework
-   **Next.js Frontend:**
    -      @apollo/client
    -      graphql
    -   axios
    -   jsonwebtoken

## Future Improvements

-   Add more advanced features to the frontend, such as user authentication and movie search.
-   Implement more robust error handling and logging in the backend.
-   Add unit and integration tests for both the backend and frontend.
-   Implement user authentication and authorization.
-   Expand the GraphQL schema to include more movie details and user-specific data.
-   Improve the UI/UX of the frontend.
-   Add CI/CD pipelines for automated deployment.

## .NET Web API Backend Setup

1.  **Create the .NET Web API Project:**

    ```bash
    mkdir backend && cd backend
    dotnet new webapi -n StreamBuddy.API
    cd StreamBuddy.API
    ```

2.  **Add Required Packages:**

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

3.  **Configure the API (appsettings.json):**

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
        "DefaultConnection": "Host=db;Port=5432;Database=streambuddy;Username=postgres;Password=yourpassword"
      },
      "JwtSettings": {
        "Secret": "YourVeryLongSecretKeyForJWT"
      }
    }
    ```

4.  **Create the Database Context (Data/AppDbContext.cs):**

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

5.  **Create a Simple Model (Models/Movie.cs):**

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

6.  **Setup Dependency Injection in Program.cs:**

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

7.  **Create GraphQL Query (Queries/Query.cs):**

    ```csharp
    using StreamBuddy.API.Data;
    using StreamBuddy.API.Models;

    public class Query
    {
        public IQueryable<Movie> GetMovies([Service] AppDbContext context) => context.Movies;
    }
    ```

8.  **Create the Dockerfile for .NET API (backend/StreamBuddy.API/Dockerfile):**

    ```dockerfile
    FROM [mcr.microsoft.com/dotnet/aspnet:9.0](https://www.google.com/search?q=https://mcr.microsoft.com/dotnet/aspnet:9.0) AS base
    WORKDIR /app
    EXPOSE 5000
    EXPOSE 5001

    FROM [mcr.microsoft.com/dotnet/sdk:9.0](https://www.google.com/search?q=https://mcr.microsoft.com/dotnet/sdk:9.0) AS build
    WORKDIR /src
    COPY ["StreamBuddy.API/StreamBuddy.API.csproj", "StreamBuddy.API/"]
    RUN dotnet restore "StreamBuddy.API/StreamBuddy.API.csproj"
    COPY
