using BookPortal.Api.Models;

public class Purchase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Guid BookId { get; set; }
    public decimal Price { get; set; }

    public User User { get; set; }
    public Book Book { get; set; }
}