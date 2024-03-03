using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<T>> GetAll()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public async Task Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public async Task Delete(int id)
    {
        var entity = await GetById(id);
        _dbContext.Set<T>().Remove(entity);
    }

    public Task SaveAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
