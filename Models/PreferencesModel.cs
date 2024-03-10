namespace Book.App.Models;

public class PreferencesModel
{
    public int Id { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int UserId { get; set; }
    public UserModel User { get; set; } = new();
}