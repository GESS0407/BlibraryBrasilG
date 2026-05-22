namespace BookPortal.Api.Dtos;

public sealed record PurchaseRequest(Guid BookId, string UserIdentifier, int Quantity, decimal Price);

