using BookPortal.Api.Dtos;
using BookPortal.Api.Models;

namespace BookPortal.Api.Repositories;

public interface ILibraryRepository
{
    Task<IReadOnlyList<Book>> SearchAsync(CatalogQuery query);
    Task<Book?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
    Task<IReadOnlyList<Shelf>> GetShelvesAsync();
    Task<int> GetActiveLoanCountAsync(string userDocument);
    Task<Loan> CreateLoanAsync(Loan loan);
    Task<IReadOnlyList<Loan>> GetLoansByUserAsync(string userDocument);
    Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        return Task.FromResult<User?>(null);
    }
}

