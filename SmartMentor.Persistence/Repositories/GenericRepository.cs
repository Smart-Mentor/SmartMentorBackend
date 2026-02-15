
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

        public GenericRepository(
            ILogger<T> logger,
            ApplicationDbContext dbContext

            )
        {
            _logger = logger;
            _dbContext= dbContext;
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {

                _logger.LogInformation($"Adding entity of type {typeof(T).Name}");
               await _dbContext.Set<T>()
               .AddAsync(entity,cancellationToken);

        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
           return _dbContext.Set<T>()
            .AnyAsync(predicate, cancellationToken);
        }

        public async  Task<int?> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>()
            .CountAsync(predicate, cancellationToken);
        }

        public void Delete(T entity)
        {

                _logger.LogInformation($"Deleting entity of type {typeof(T).Name}");
                _dbContext.Set<T>().Remove(entity);

        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Finding entities of type {typeof(T).Name} with predicate {predicate}");
            
            return await _dbContext.Set<T>()
            .Where(predicate)
            .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Getting all entities of type {typeof(T).Name}");
             return await _dbContext.Set<T>()
             .ToListAsync(cancellationToken);

        }

        public async Task<T?> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
                _logger.LogInformation("Getting entity of type {EntityType} by id {KeyValues}", 
                typeof(T).Name, string.Join(",", keyValues));

                return await _dbContext.Set<T>()
                .FindAsync(keyValues: keyValues, cancellationToken: cancellationToken);
        }

        public void Update(T entity)
        {
             _logger.LogInformation($"Updating entity of type {typeof(T).Name}");

                _dbContext.Set<T>()
                .Update(entity);   
        }

    }
}   


