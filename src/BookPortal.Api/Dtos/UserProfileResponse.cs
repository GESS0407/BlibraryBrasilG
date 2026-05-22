using BookPortal.Api.Data;

namespace BookPortal.Api.Dtos;

public sealed record UserProfileResponse(
    int Id,
    string Name,
    string Cpf,
    string Email,
    IReadOnlyList<UserLoanBookResponse> BorrowedBooks,
    IReadOnlyList<UserPurchaseBookResponse> PurchasedBooks)
{
    public static UserProfileResponse FromData(
        User user,
        IReadOnlyList<UserLoanBookResponse> borrowedBooks,
        IReadOnlyList<UserPurchaseBookResponse> purchasedBooks) =>
        new(user.Id, user.Name, user.Cpf, user.Email, borrowedBooks, purchasedBooks);
}

public sealed record UserLoanBookResponse(
    Guid LoanId,
    Guid BookId,
    string Title,
    string Author,
    string Category,
    string Format,
    DateTimeOffset BorrowedAt,
    DateTimeOffset DueAt,
    DateTimeOffset? ReturnedAt)
{
    public static UserLoanBookResponse FromData(LoanEntity loan, BookEntity book) =>
        new(
            loan.Id,
            book.Id,
            book.Title,
            book.Author,
            book.Category,
            book.Format,
            loan.BorrowedAt,
            loan.DueAt,
            loan.ReturnedAt);
}

public sealed record UserPurchaseBookResponse(
    int PurchaseId,
    Guid BookId,
    string Title,
    string Author,
    string Category,
    string Format,
    decimal Price,
    int Quantity,
    DateTime CreatedAt)
{
    public static UserPurchaseBookResponse FromData(Purchase purchase, BookEntity book) =>
        new(
            purchase.Id,
            book.Id,
            book.Title,
            book.Author,
            book.Category,
            book.Format,
            purchase.Price,
            purchase.Quantity,
            purchase.CreatedAt);
}

