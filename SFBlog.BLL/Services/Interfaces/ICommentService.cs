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
    public interface ICommentService
    {
        Task<EntityBaseResponse<CommentDomain>> Add(CommentDomain commentDomain);

        Task<EntityBaseResponse<CommentDomain>> Delete(CommentDomain commentDomain);

        Task<EntityBaseResponse<CommentDomain>> Update(CommentDomain commentDomain);

        Task<EntityBaseResponse<CommentDomain>> Get(int id);

        Task<EntityBaseResponse<IEnumerable<CommentDomain>>> GetByPost(int postId);

        EntityBaseResponse<IEnumerable<CommentDomain>> GetAll();

    }
}
