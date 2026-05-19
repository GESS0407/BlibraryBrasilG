using System.Text.Json;
using BookPortal.Api.Models;

namespace BookPortal.Api.Data;

public sealed class BookEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string AgeRange { get; set; } = string.Empty;
    public string CoverColor { get; set; } = "#315f72";
    public string CoverAccent { get; set; } = "#f2c94c";
    public string TagsJson { get; set; } = "[]";
    public int CopiesAvailable { get; set; }
    public DateTimeOffset AddedAt { get; set; }

    public Book ToModel()
    {
        var tags = JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();

        return new Book
        {
            Id = Id,
            Title = Title,
            Author = Author,
            Description = Description,
            Category = Category,
            Format = Format,
            Language = Language,
            AgeRange = AgeRange,
            CoverColor = CoverColor,
            CoverAccent = CoverAccent,
            Tags = tags,
            CopiesAvailable = CopiesAvailable,
            AddedAt = AddedAt
        };
    }

    public static BookEntity FromModel(Book book) =>
        new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Category = book.Category,
            Format = book.Format,
            Language = book.Language,
            AgeRange = book.AgeRange,
            CoverColor = book.CoverColor,
            CoverAccent = book.CoverAccent,
            TagsJson = JsonSerializer.Serialize(book.Tags),
            CopiesAvailable = book.CopiesAvailable,
            AddedAt = book.AddedAt
        };
}

