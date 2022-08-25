using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SFBlog.DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _db;

        public DbSet<TEntity> Set { get; private set; }

        public Repository(SFBlogDbContext db)
        {
            _db = db;
            var set = _db.Set<TEntity>();
            set.Load();
            Set = set;
        }

        public async Task<int> Create(TEntity item)
        {
            Set.Add(item);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> Delete(TEntity item)
        {
            Set.Remove(item);
            return await _db.SaveChangesAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            TEntity entity = await Set.FindAsync(id);
            return entity;
        }

        public async virtual Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (string includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Set;
        }

        public async Task<int> Update(TEntity item)
        {
            Set.Update(item);
            return await _db.SaveChangesAsync();
        }
    }
}
