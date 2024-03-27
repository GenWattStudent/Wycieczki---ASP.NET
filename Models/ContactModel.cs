using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class ContactModel : BaseEntity
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public int Phone { get; set; }
    public int UserId { get; set; }
    public UserModel User { get; set; } = new();

    public ContactModel(Contact contact)
    {
        Email = contact.Email;
        Phone = contact.Phone;
    }

    public ContactModel()
    {
    }

    public void SetContact(Contact contact)
    {
        Email = contact.Email;
        Phone = contact.Phone;
    }
}