using BookPortal.Api.Models;

namespace BookPortal.Api.Dtos;

public sealed record LoanResponse(
    Guid Id,
    Guid BookId,
    string? BookTitle,
    int UserId,
    string? UserName,
    DateTimeOffset BorrowedAt,
    DateTimeOffset DueAt,
    DateTimeOffset? ReturnedAt)
{
    public static LoanResponse FromModel(Loan loan) =>
        new(
            loan.Id,
            loan.BookId,
            loan.Book?.Title,
            loan.UserId,
            loan.User?.Name,
            loan.BorrowedAt,
            loan.DueAt,
            loan.ReturnedAt);
}

