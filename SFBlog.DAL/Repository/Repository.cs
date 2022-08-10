using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFBlog.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext _db;

        public DbSet<T> Set { get; private set; }

        public Repository(SFBlogDbContext db)
        {
            _db = db;
            var set = _db.Set<T>();
            set.Load();
            Set = set;
        }

        public async Task<int> Create(T item)
        {
            Set.Add(item);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> Delete(T item)
        {
            Set.Remove(item);
            return await _db.SaveChangesAsync();
        }

        public async Task<T> Get(int id)
        {
            T entity = await Set.FindAsync(id);
            return entity;
        }

        public IEnumerable<T> GetAll()
        {
            return Set;
        }

        public async Task<int> Update(T item)
        {
            Set.Update(item);
            return await _db.SaveChangesAsync();
        }
    }
}
