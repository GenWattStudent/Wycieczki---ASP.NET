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

    public async Task<List<string>> SaveFiles(List<IFormFile> files, string folder)
    {
        var imageUrls = new List<string>();

        foreach (var file in files)
        {
            var path = await SaveFile(file, folder);
            imageUrls.Add(path);
        }

        return imageUrls;
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
