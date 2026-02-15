using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Persistence.Data;

namespace SmartMentor.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(
            ILogger<UnitOfWork> logger,
            ILoggerFactory loggerFactory,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _dbContext = dbContext;
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (_repositories.TryGetValue(type, out var repo))
            {
                return (IGenericRepository<T>)repo;
            }

            var logger = _loggerFactory.CreateLogger<T>();
            var newRepo = new GenericRepository<T>(logger, _dbContext);
            _repositories[type] = newRepo;
            return newRepo;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to the database");
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

    }

}