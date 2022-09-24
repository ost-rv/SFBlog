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
    public interface IRoleService
    {
        Task<EntityBaseResponse<RoleDomain>> Add(RoleDomain roleDomain);

        Task<EntityBaseResponse<RoleDomain>> Delete(RoleDomain roleDomain);

        Task<EntityBaseResponse<RoleDomain>> Update(RoleDomain roleDomain);

        Task<EntityBaseResponse<RoleDomain>> Get(int id);

        EntityBaseResponse<IEnumerable<RoleDomain>> GetAll();

    }
}
