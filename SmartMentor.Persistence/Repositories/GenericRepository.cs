
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Persistence.Data;

namespace SmartMentor.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ILogger<T> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(
            ILogger<T> logger,
            ApplicationDbContext dbContext

            )
        {
            _logger = logger;
            _dbContext= dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {

                _logger.LogInformation($"Adding entity of type {typeof(T).Name}");
               await _dbSet
               .AddAsync(entity,cancellationToken);

        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
           return _dbSet
            .AnyAsync(predicate, cancellationToken);
        }

        public async  Task<int?> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
            .CountAsync(predicate, cancellationToken);
        }

        public void Delete(T entity)
        {

                _logger.LogInformation($"Deleting entity of type {typeof(T).Name}");
                _dbSet.Remove(entity);

        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Finding entities of type {typeof(T).Name} with predicate {predicate}");
            
            return await _dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Getting all entities of type {typeof(T).Name}");
             return await _dbSet
             .ToListAsync(cancellationToken);

        }

        public async Task<T?> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
                _logger.LogInformation("Getting entity of type {EntityType} by id {KeyValues}", 
                typeof(T).Name, string.Join(",", keyValues));

                return await _dbSet
                .FindAsync(keyValues: keyValues, cancellationToken: cancellationToken);
        }

        public void Update(T entity)
        {
             _logger.LogInformation($"Updating entity of type {typeof(T).Name}");

                _dbSet
                .Update(entity);   
        }

    }
}   


