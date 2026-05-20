using BookPortal.Api.Data;
using BookPortal.Api.Dtos;
using BookPortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookPortal.Api.Repositories;

public sealed class SqliteLibraryRepository : ILibraryRepository
{
    private readonly LibraryDbContext _db;

    public SqliteLibraryRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Book>> SearchAsync(CatalogQuery query)
    {
        IQueryable<BookEntity> result = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            result = result.Where(book =>
                EF.Functions.Like(book.Title, $"%{term}%") ||
                EF.Functions.Like(book.Author, $"%{term}%") ||
                EF.Functions.Like(book.Description, $"%{term}%") ||
                EF.Functions.Like(book.TagsJson, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            result = result.Where(book => book.Category == query.Category);
        }

        if (!string.IsNullOrWhiteSpace(query.Format))
        {
            result = result.Where(book => book.Format == query.Format);
        }

        if (query.Available is true)
        {
            result = result.Where(book => book.CopiesAvailable > 0);
        }

        var books = await result.OrderBy(book => book.Title).ToListAsync();
        return books.Select(book => book.ToModel()).ToList();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        var book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        return book?.ToModel();
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync() =>
        await _db.Books
            .AsNoTracking()
            .Select(book => book.Category)
            .Distinct()
            .OrderBy(category => category)
            .ToListAsync();

    public async Task<IReadOnlyList<Shelf>> GetShelvesAsync()
    {
        var allBooks = await _db.Books.AsNoTracking().ToListAsync();
        var books = allBooks.Select(book => book.ToModel()).ToList();

        return
        [
            new Shelf
            {
                Name = "Novidades do acervo",
                Description = "Titulos adicionados recentemente para leitura digital.",
                Books = books.OrderByDescending(book => book.AddedAt).Take(6).ToList()
            },
            new Shelf
            {
                Name = "Classicos essenciais",
                Description = "Obras de referencia para repertorio literario.",
                Books = books.Where(book => book.Tags.Contains("classico")).Take(6).ToList()
            },
            new Shelf
            {
                Name = "Para usar em sala",
                Description = "Selecao pensada para projetos, debates e sequencias didaticas.",
                Books = books.Where(book => book.Tags.Contains("sala-de-aula")).Take(6).ToList()
            }
        ];
    }

    public async Task<int> GetActiveLoanCountAsync(string userDocument)
    {
        var cpf = userDocument.Trim();

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Cpf == cpf);
        if (user is null)
        {
            return 0;
        }

        return await _db.Loans.CountAsync(loan =>
            loan.UserId == user.Id && 
            loan.ReturnedAt == null);
    }

    public async Task<Loan> CreateLoanAsync(Loan loan)
    {
        var book = await _db.Books.FirstAsync(item => item.Id == loan.BookId);
        book.CopiesAvailable = Math.Max(0, book.CopiesAvailable - 1);

        _db.Loans.Add(LoanEntity.FromModel(loan));
        await _db.SaveChangesAsync();

        return loan;
    }

    public async Task<IReadOnlyList<Loan>> GetLoansByUserAsync(string userDocument)
    {
        var cpf = userDocument.Trim();

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Cpf == cpf);

        if (user is null)
        {
            return [];
        }

        var loans = await _db.Loans
            .AsNoTracking()
            .Where(loan => loan.UserId == user.Id)
            .OrderByDescending(loan => loan.BorrowedAt)
            .ToListAsync();

        return loans.Select(loan => loan.ToModel()).ToList();
    }

    public async Task<User?> GetUserByCpfAsync(string cpf)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Cpf == cpf.Trim());
    }   
}

