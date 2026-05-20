public class User
{
    public int Id {get; set;}
    public required string Name {get; set;}
    public required string Cpf {get; set;}
    public required string Email{get; set;}
    public required string Password{get; set;}
    public DateTime CreatedAT { get; set;} = DateTime.Now;
}