using BookPortal.Api.Data;
using BookPortal.Api.Dtos;
using BookPortal.Api.Models;

namespace BookPortal.Api.Repositories;

public sealed class InMemoryLibraryRepository : ILibraryRepository
{
    private readonly List<Book> _books = SeedData.Books.ToList();
    private readonly List<Loan> _loans = [];
    private readonly object _gate = new();

    public Task<IReadOnlyList<Book>> SearchAsync(CatalogQuery query)
    {
        IEnumerable<Book> result = _books;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            result = result.Where(book =>
                book.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                book.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                book.Tags.Any(tag => tag.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            result = result.Where(book => book.Category.Equals(query.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Format))
        {
            result = result.Where(book => book.Format.Equals(query.Format, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Available is true)
        {
            result = result.Where(book => book.CopiesAvailable > 0);
        }

        return Task.FromResult<IReadOnlyList<Book>>(result.OrderBy(book => book.Title).ToList());
    }

    public Task<Book?> GetByIdAsync(Guid id) =>
        Task.FromResult(_books.FirstOrDefault(book => book.Id == id));

    public Task<IReadOnlyList<string>> GetCategoriesAsync() =>
        Task.FromResult<IReadOnlyList<string>>(_books.Select(book => book.Category).Distinct().Order().ToList());

    public Task<IReadOnlyList<Shelf>> GetShelvesAsync()
    {
        var newest = _books.OrderByDescending(book => book.AddedAt).Take(6).ToList();
        var classics = _books.Where(book => book.Tags.Contains("classico")).Take(6).ToList();
        var classroom = _books.Where(book => book.Tags.Contains("sala-de-aula")).Take(6).ToList();

        IReadOnlyList<Shelf> shelves =
        [
            new Shelf
            {
                Name = "Novidades do acervo",
                Description = "Titulos adicionados recentemente para leitura digital.",
                Books = newest
            },
            new Shelf
            {
                Name = "Classicos essenciais",
                Description = "Obras de referencia para repertorio literario.",
                Books = classics
            },
            new Shelf
            {
                Name = "Para usar em sala",
                Description = "Selecao pensada para projetos, debates e sequencias didaticas.",
                Books = classroom
            }
        ];

        return Task.FromResult(shelves);
    }

    public Task<int> GetActiveLoanCountAsync(string userDocument)
    {
        lock (_gate)
        {
            return Task.FromResult(_loans.Count(loan =>
                loan.UserId.ToString() == userDocument.Trim() &&
                loan.ReturnedAt is null));
        }
    }

    public Task<Loan> CreateLoanAsync(Loan loan)
    {
        lock (_gate)
        {
            var index = _books.FindIndex(book => book.Id == loan.BookId);
            if (index >= 0)
            {
                var current = _books[index];
                _books[index] = current with { CopiesAvailable = Math.Max(0, current.CopiesAvailable - 1) };
            }

            _loans.Add(loan);
            return Task.FromResult(loan);
        }
    }

    public Task<IReadOnlyList<Loan>> GetLoansByUserAsync(string userDocument)
    {
        lock (_gate)
        {
            var loans = _loans
                .Where(loan => loan.UserId.ToString() == userDocument.Trim())
                .OrderByDescending(loan => loan.BorrowedAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<Loan>>(loans);
        }
    }
}

