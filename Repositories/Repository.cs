using System.Linq.Expressions;
using Book.App.Models;
using Book.App.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
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

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public async Task Remove(int id)
    {
        var entity = await GetById(id);

        if (entity == null)
        {
            return;
        }

        _dbContext.Set<T>().Remove(entity);
    }

    public Task SaveAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Task<T?> GetSingleBySpec(ISpecification<T> spec)
    {
        return ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public Task<List<T>> GetBySpec(ISpecification<T> spec)
    {
        return ApplySpecification(spec).ToListAsync();
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().AddRange(entities);
    }

    public void Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public bool Contains(Expression<Func<T, bool>> predicate)
    {
        return _dbContext.Set<T>().Any(predicate);
    }

    public bool Contains(ISpecification<T> spec)
    {
        return ApplySpecification(spec).Any();
    }

    public int Count(Expression<Func<T, bool>> predicate)
    {
        return _dbContext.Set<T>().Count(predicate);
    }

    public int Count(ISpecification<T> spec)
    {
        return ApplySpecification(spec).Count();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    }
}
