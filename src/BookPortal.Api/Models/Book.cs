namespace BookPortal.Api.Models;

public sealed record Book
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public required string Format { get; init; }
    public required string Language { get; init; }
    public required string AgeRange { get; init; }
    public required string CoverColor { get; init; }
    public required string CoverAccent { get; init; }
    public required List<string> Tags { get; init; }
    public int CopiesAvailable { get; init; }
    public DateTimeOffset AddedAt { get; init; }
}
