using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SFBlog.DAL.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Get(int id);

        public Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);

        public Task<TEntity> LoadNavigateProperty(TEntity item, params Expression<Func<TEntity, object>>[] includes);

        Task<int> Create(TEntity item);
        Task<int> Update(TEntity item);
        Task<int> Delete(TEntity item);
    }
}
