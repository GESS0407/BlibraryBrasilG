# Biblioteca Digital

An original digital-library website project inspired by the public structure of MEC Livros: catalog discovery, curated shelves, book details, and a borrowing flow.

## Stack

- Backend: ASP.NET Core minimal API in C#
- Frontend: Vanilla JavaScript, HTML, CSS served from `wwwroot`
- Data: SQLite through Entity Framework Core, with seed data on startup

## Project Layout

```text
src/
  BookPortal.Api/
    BookPortal.Api.csproj
    BookPortal.Api.http
    Program.cs
    appsettings.json
    appsettings.Development.json
    Properties/launchSettings.json
    Data/
    Dtos/
    Models/
    Repositories/
    Services/
    wwwroot/
```

## Run

Install the .NET SDK, then:

```powershell
dotnet restore .\src\BookPortal.Api\BookPortal.Api.csproj
dotnet run --project .\src\BookPortal.Api\BookPortal.Api.csproj
```

Open the URL printed by `dotnet run`.

You can also open `BookPortal.sln` in Visual Studio.

## Static Frontend Preview

The frontend can be previewed without the .NET SDK:

```powershell
node .\scripts\static-preview-server.js .\src\BookPortal.Api\wwwroot 5177
```

Open `http://127.0.0.1:5177/`. API requests will fall back to local preview data.

## SQLite Integration

The API uses `LibraryDbContext` and `SqliteLibraryRepository` by default.
The current configuration stores data in `library.db`.

To create or update the database schema manually:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialLibrarySchema --project .\src\BookPortal.Api\BookPortal.Api.csproj
dotnet ef database update --project .\src\BookPortal.Api\BookPortal.Api.csproj
```

When `Database:SeedOnStartup` is `true`, the app inserts the sample catalog if the `Books` table is empty.
