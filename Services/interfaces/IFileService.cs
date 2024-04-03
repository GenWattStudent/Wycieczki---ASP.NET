namespace Book.App.Services;

public interface IFileService
{
    Task<string> SaveFile(IFormFile file, string folder);
    Task<List<string>> SaveFiles(List<IFormFile> files, string folder);
    Task DeleteFile(string path);
}