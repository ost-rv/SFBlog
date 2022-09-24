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
    public interface ITagService
    {
        Task<EntityBaseResponse<TagDomain>> Add(TagDomain tagDomain);

        Task<EntityBaseResponse<TagDomain>> Delete(TagDomain tagDomain);

        Task<EntityBaseResponse<TagDomain>> Update(TagDomain tagDomain);

        Task<EntityBaseResponse<TagDomain>> Get(int id);

        EntityBaseResponse<IEnumerable<TagDomain>> GetAll();

    }
}
