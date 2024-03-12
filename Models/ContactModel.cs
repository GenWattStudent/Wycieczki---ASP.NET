using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class ContactModel
{
    public int Id { get; set; }
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public int Phone { get; set; }
    public int UserId { get; set; }
    public UserModel User { get; set; } = new();
}