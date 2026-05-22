using BookPortal.Api.Data;

public class Purchase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public Guid BookId { get; set; }
    public BookEntity? Book { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
