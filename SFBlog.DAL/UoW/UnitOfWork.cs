using Microsoft.EntityFrameworkCore.Infrastructure;
using SFBlog.DAL.Repository;
using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;

namespace SFBlog.DAL.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private SFBlogDbContext _dbContext;

        private Dictionary<Type, object> _repositories;
        private Dictionary<Type, object> _customRepositories = new Dictionary<Type, object>();

        public UnitOfWork(SFBlogDbContext dbContext)
        {
            _dbContext = dbContext;
            _customRepositories.Add(typeof(User), new UserRepository(_dbContext));
        }

        public void Dispose()
        {

        }

        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }
           
            if (hasCustomRepository)
            {
                var modelType = typeof(TEntity);
                if (_customRepositories.ContainsKey(modelType))
                {
                    return (IRepository<TEntity>)_customRepositories[modelType];
                }
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(_dbContext);
            }

            return (IRepository<TEntity>)_repositories[type];

        }
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }
    }
}