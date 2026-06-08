# Biblioteca Digital

Um projeto original de site de biblioteca digital inspirado na estrutura pública do MEC Livros: descoberta de catálogo, estantes com curadoria, detalhes do livro e fluxo de empréstimo.

## Stack

- Backend: ASP.NET Core minimal API em C#
- Frontend: Vanilla JavaScript, HTML, CSS servidos a partir de `wwwroot`
- Dados: SQLite através do Entity Framework Core, com dados iniciais (seed data) na inicialização

## Estrutura do Projeto

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

## Como Executar

Instale o .NET SDK e, em seguida:

```powershell
dotnet restore .\src\BookPortal.Api\BookPortal.Api.csproj
dotnet run --project .\src\BookPortal.Api\BookPortal.Api.csproj
```

Abra a URL exibida pelo `dotnet run`.

Você também pode abrir o `BookPortal.sln` no Visual Studio.

## Visualização Estática do Frontend

O frontend pode ser pré-visualizado sem o .NET SDK:

```powershell
node .\scripts\static-preview-server.js .\src\BookPortal.Api\wwwroot 5177
```

Abra `http://127.0.0.1:5177/`. As requisições de API usarão dados de pré-visualização locais como alternativa (fallback).

## Integração com SQLite

A API usa `LibraryDbContext` e `SqliteLibraryRepository` por padrão.
A configuração atual armazena os dados em `library.db`.

Para criar ou atualizar o esquema do banco de dados manualmente:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialLibrarySchema --project .\src\BookPortal.Api\BookPortal.Api.csproj
dotnet ef database update --project .\src\BookPortal.Api\BookPortal.Api.csproj
```

Quando `Database:SeedOnStartup` for `true`, o aplicativo insere o catálogo de exemplo se a tabela `Books` estiver vazia.
