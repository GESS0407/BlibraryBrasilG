using BookPortal.Api.Data;
using BookPortal.Api.Dtos;
using BookPortal.Api.Models;
using BookPortal.Api.Repositories;
using BookPortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<LibraryOptions>(builder.Configuration.GetSection("Library"));
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddScoped<BorrowingService>();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Library")));

builder.Services.AddScoped<ILibraryRepository, SqliteLibraryRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();
Console.WriteLine("ContentRoot: " + app.Environment.ContentRootPath);
Console.WriteLine("ConnectionString: " + builder.Configuration.GetConnectionString("Library"));

await app.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new { status = "ok", service = "digital-library" }));

api.MapGet("/catalog", async (
    ILibraryRepository repository,
    string? q,
    string? category,
    string? format,
    bool? available) =>
{
    var books = await repository.SearchAsync(new CatalogQuery(q, category, format, available));
    return Results.Ok(books.Select(BookResponse.FromModel));
});

api.MapGet("/catalog/{id:guid}", async Task<IResult> (Guid id, ILibraryRepository repository) =>
{
    var book = await repository.GetByIdAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(BookResponse.FromModel(book));
});

api.MapGet("/categories", async (ILibraryRepository repository) =>
{
    var categories = await repository.GetCategoriesAsync();
    return Results.Ok(categories);
});

api.MapGet("/shelves", async (ILibraryRepository repository) =>
{
    var shelves = await repository.GetShelvesAsync();
    return Results.Ok(shelves.Select(ShelfResponse.FromModel));
});

api.MapPost("/loans", async Task<IResult> (
    BorrowRequest request,
    BorrowingService borrowingService) =>
{
    var result = await borrowingService.BorrowAsync(request);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(new { error = result.Error });
    }

    return Results.Created($"/api/users/{request.UserDocument}/loans", LoanResponse.FromModel(result.Loan!));
});

api.MapGet("/users/{document}/loans", async (string document, ILibraryRepository repository) =>
{
    var loans = await repository.GetLoansByUserAsync(document);
    return Results.Ok(loans.Select(LoanResponse.FromModel));
});

api.MapPost("/users", async Task<IResult>(
    [FromBody] User user,
    [FromServices] LibraryDbContext db) =>
{
    var cpf = user.Cpf.Trim();
    var email = user.Email.Trim();

    var exists = await db.Users.AnyAsync(u => u.Cpf == cpf || u.Email == email);

    if (exists)
    {
        return Results.BadRequest(new { error = "Usuario já cadastrado com este CPF ou email."});
    }

    user.Cpf = cpf;
    user.Email = email;

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/(user.Cpf)", new
    {
        user.Id,
        user.Name,
        user.Cpf,
        user.Email
    });
});


app.MapFallbackToFile("index.html");

app.Run();
