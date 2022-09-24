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
    public interface IPostService
    {
        Task<EntityBaseResponse<PostDomain>> Add(PostDomain postDomain);

        Task<EntityBaseResponse<PostDomain>> Delete(PostDomain postDomain);

        Task<EntityBaseResponse<PostDomain>> Update(PostDomain postDomain);

        Task<EntityBaseResponse<PostDomain>> Get(int id);

        EntityBaseResponse<IEnumerable<PostDomain>> GetAll();

        IEnumerable<TagDomain> GetAllTags();


    }
}
