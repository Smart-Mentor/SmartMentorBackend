using System.Linq.Expressions;

namespace SmartMentor.Abstraction.Repositories
{
    public interface IGenericRepository<T>  where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(object[]keyvalues, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int?> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    }
}