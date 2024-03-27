using System.Linq.Expressions;
using Book.App.Models;
using Book.App.Specifications;

namespace Book.App.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    // Get
    Task<List<T>> GetAll();
    Task<T?> GetById(int id);
    Task<T?> GetSingleBySpec(ISpecification<T> spec);
    Task<List<T>> GetBySpec(ISpecification<T> spec);
    // Add
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    // Update
    void Update(T entity);
    // Remove
    void Remove(T entity);
    Task Remove(int id);
    void RemoveRange(IEnumerable<T> entities);
    // Contains
    bool Contains(Expression<Func<T, bool>> predicate);
    bool Contains(ISpecification<T> spec);
    // Count    
    int Count(Expression<Func<T, bool>> predicate);
    int Count(ISpecification<T> spec);
    // Save
    Task SaveAsync();
}