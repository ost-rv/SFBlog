using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.DAL.Repository
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(SFBlogDbContext db) : base(db)
        {

        }

        public User GetByLogin(string login)
        {
            return Set.FirstOrDefault(u => u.Login == login);
        }
    }
}
