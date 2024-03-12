namespace Book.App.Models;

public class PreferencesModel
{
    public int Id { get; set; }
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "en";
    public bool IsPublic { get; set; } = true;
    public int UserId { get; set; }
    public UserModel User { get; set; }
}