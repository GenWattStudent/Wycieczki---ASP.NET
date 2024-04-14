using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class ContactModel : BaseEntity
{
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;
    public int Phone { get; set; }
    public UserModel? User { get; set; }

    public ContactModel(ContactModel contact)
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