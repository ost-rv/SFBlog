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

        public async Task<TEntity> LoadNavigateProperty(TEntity item, params Expression<Func<TEntity, object>>[] includes)
        {
            if (_db.Entry(item).State == EntityState.Detached)
                return null;

            if (includes.Length == 0)
            {
                foreach (var navProp in _db.Entry(item).Navigations)
                {
                    await navProp.LoadAsync();
                }
            }
            else 
            {
                MemberExpression expressionBody = null;
                string propName = String.Empty;
                foreach (var expression in includes)
                {
                    expressionBody = (MemberExpression)expression.Body;
                    propName = expressionBody.Member.Name;
                    await _db.Entry(item).Navigation(propName).LoadAsync();
                }
            }
            return item;
        }

        public async virtual Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (Expression<Func<TEntity, object>> expression in includes)
            {
                query = query.Include(expression);
            }

            if (orderBy != null)
            {
                return  await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Set;
            foreach (Expression<Func<TEntity, object>> expression in includes)
            {
                query = query.Include(expression);
            }
            query.Load();
            return Set;
        }

        public async Task<int> Update(TEntity item)
        {
            Set.Update(item);
            return await _db.SaveChangesAsync();
        }

    }
}
