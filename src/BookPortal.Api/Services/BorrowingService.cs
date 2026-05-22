using BookPortal.Api.Dtos;
using BookPortal.Api.Models;
using BookPortal.Api.Repositories;
using Microsoft.Extensions.Options;

namespace BookPortal.Api.Services;

public sealed class BorrowingService
{
    private readonly ILibraryRepository _repository;
    private readonly LibraryOptions _options;

    public BorrowingService(ILibraryRepository repository, IOptions<LibraryOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public async Task<BorrowResult> BorrowAsync(BorrowRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserDocument))
        {
            return BorrowResult.Failure("Informe um documento de usuario para reservar a obra.");
        }
        var user = await _repository.GetUserByIdentifierAsync(request.UserDocument);
        
        if (user is null)
        {
            return BorrowResult.Failure("Usuario nao encontrado.");
        }

        var book = await _repository.GetByIdAsync(request.BookId);
        if (book is null)
        {
            return BorrowResult.Failure("Obra nao encontrada.");
        }

        if (book.CopiesAvailable <= 0)
        {
            return BorrowResult.Failure("Esta obra nao esta disponivel para emprestimo no momento.");
        }

        var activeLoans = await _repository.GetActiveLoanCountAsync(request.UserDocument);
        if (activeLoans >= _options.MaxMonthlyLoans)
        {
            return BorrowResult.Failure($"Limite de {_options.MaxMonthlyLoans} emprestimos ativos atingido.");
        }

        var now = DateTimeOffset.UtcNow;
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            UserId = user.Id,
            BorrowedAt = now,
            DueAt = now.AddDays(_options.LoanDays)
        };

        var created = await _repository.CreateLoanAsync(loan);
        return BorrowResult.Success(created);
    }
}

