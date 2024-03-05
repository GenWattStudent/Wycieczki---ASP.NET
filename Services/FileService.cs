namespace Book.App.Services;

public class FileService
{
    public async Task<string> SaveFile(IFormFile file, string folder)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder, fileName);

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/{folder}/{fileName}";
    }

    public async Task DeleteFile(string path)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
