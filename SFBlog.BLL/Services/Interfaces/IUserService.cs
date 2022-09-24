using SFBlog.BLL.Models;
using SFBlog.BLL.Response;
using SFBlog.DAL.Models;
using SFBlog.DAL.Repository;
using SFBlog.DAL.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Services
{
    public interface IUserService
    {
        Task<EntityBaseResponse<UserDomain>> Register(UserDomain userDomain);

        EntityBaseResponse<UserDomain> Authenticate(UserAuthenticateDomain userAuthenticateDomain);

        EntityBaseResponse<UserDomain> GetByLogin(string login);

        Task<EntityBaseResponse<UserDomain>> Get(int id);

        Task<EntityBaseResponse<UserDomain>> Delete(UserDomain userDomain);

        Task<EntityBaseResponse<UserDomain>> Update(UserDomain userDomain);

        EntityBaseResponse<IEnumerable<UserDomain>> GetAll();

        IEnumerable<RoleDomain> GetAllRoles();


    }
}
