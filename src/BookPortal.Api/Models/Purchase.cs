using BookPortal.Api.Models;

public class Purchase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User {get; set;}
    public Guid BookId{get; set;}
    public Book? Book {get; set;}
    public decimal Price { get; set; }
    public int Quantity {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now; 

}