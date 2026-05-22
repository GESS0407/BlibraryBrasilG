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

api.MapPost("/auth/login", async Task<IResult> (
    UserLoginRequest request,
    LibraryDbContext db) =>
{
    var user = await FindUserByIdentifierAsync(db, request.Identifier);
    if (user is null)
    {
        return Results.NotFound(new { error = "Usuario nao encontrado." });
    }

    return Results.Ok(new
    {
        user.Id,
        user.Name,
        user.Cpf,
        user.Email
    });
});

api.MapGet("/users/profile", async Task<IResult> (
    string identifier,
    LibraryDbContext db) =>
{
    var user = await FindUserByIdentifierAsync(db, identifier);
    if (user is null)
    {
        return Results.NotFound(new { error = "Usuario nao encontrado." });
    }

    var borrowedRows = await (
        from loan in db.Loans.AsNoTracking()
        join book in db.Books.AsNoTracking() on loan.BookId equals book.Id
        where loan.UserId == user.Id
        select new { Loan = loan, Book = book })
        .ToListAsync();

    var borrowedBooks = borrowedRows
        .OrderByDescending(row => row.Loan.BorrowedAt)
        .Select(row => UserLoanBookResponse.FromData(row.Loan, row.Book))
        .ToList();

    var purchasedRows = await (
        from purchase in db.Purchases.AsNoTracking()
        join book in db.Books.AsNoTracking() on purchase.BookId equals book.Id
        where purchase.UserId == user.Id
        select new { Purchase = purchase, Book = book })
        .ToListAsync();

    var purchasedBooks = purchasedRows
        .OrderByDescending(row => row.Purchase.CreatedAt)
        .Select(row => UserPurchaseBookResponse.FromData(row.Purchase, row.Book))
        .ToList();

    return Results.Ok(UserProfileResponse.FromData(user, borrowedBooks, purchasedBooks));
});

api.MapPost("/purchases", async Task<IResult> (
    PurchaseRequest request,
    LibraryDbContext db) =>
{
    var user = await FindUserByIdentifierAsync(db, request.UserIdentifier);
    if (user is null)
    {
        return Results.NotFound(new { error = "Usuario nao encontrado." });
    }

    var book = await db.Books.FirstOrDefaultAsync(book => book.Id == request.BookId);
    if (book is null)
    {
        return Results.NotFound(new { error = "Obra nao encontrada." });
    }

    var purchase = new Purchase
    {
        UserId = user.Id,
        BookId = book.Id,
        Price = Math.Max(0, request.Price),
        Quantity = Math.Max(1, request.Quantity)
    };

    db.Purchases.Add(purchase);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/users/profile?identifier={Uri.EscapeDataString(user.Cpf)}",
        UserPurchaseBookResponse.FromData(purchase, book));
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
        return Results.BadRequest(new { error = "Usuario ja cadastrado com este CPF ou email." });
    }

    user.Cpf = cpf;
    user.Email = email;

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Cpf}", new
    {
        user.Id,
        user.Name,
        user.Cpf,
        user.Email
    });
});


app.MapFallbackToFile("index.html");

app.Run();

static Task<User?> FindUserByIdentifierAsync(LibraryDbContext db, string identifier)
{
    var normalizedIdentifier = identifier.Trim();
    var normalizedEmail = normalizedIdentifier.ToLower();

    return db.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(user =>
            user.Cpf == normalizedIdentifier ||
            user.Email.ToLower() == normalizedEmail);
}
