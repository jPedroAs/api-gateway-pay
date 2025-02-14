using System.ComponentModel.DataAnnotations;

public class User
{
    public User(){
        Username = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
    }
    public int Id { get; set; }
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; } 
}