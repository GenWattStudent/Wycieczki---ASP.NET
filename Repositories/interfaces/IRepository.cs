namespace Book.App.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAll();
    Task<T?> GetById(int id);
    void Add(T entity);
    void Update(T entity);
    Task Delete(int id);
    Task SaveAsync();
}