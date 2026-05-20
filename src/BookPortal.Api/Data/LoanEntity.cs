using BookPortal.Api.Models;

namespace BookPortal.Api.Data;

public sealed class LoanEntity
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int UserId{get; set;}

    public DateTimeOffset BorrowedAt { get; set; }
    public DateTimeOffset DueAt { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }

    public Loan ToModel() =>
        new()
        {
            Id = Id,
            BookId = BookId,
            UserId = UserId,
            BorrowedAt = BorrowedAt,
            DueAt = DueAt,
            ReturnedAt = ReturnedAt
        };

    public static LoanEntity FromModel(Loan loan) =>
        new()
        {
            Id = loan.Id,
            BookId = loan.BookId,
            UserId = loan.UserId,
            BorrowedAt = loan.BorrowedAt,
            DueAt = loan.DueAt,
            ReturnedAt = loan.ReturnedAt
        };
}

