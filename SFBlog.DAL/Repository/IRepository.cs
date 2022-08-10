using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFBlog.DAL.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T> Get(int id);
        Task<int> Create(T item);
        Task<int> Update(T item);
        Task<int> Delete(T item);
    }
}
