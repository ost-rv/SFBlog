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
    public class CommentService : ICommentService
    {
        private IUnitOfWork _UoW;
        private Repository<Comment> _commentRepository;
        private Repository<User> _userRepository;
        private Repository<Post> _postRepository;


        public CommentService(IUnitOfWork UoW)
        {
            _UoW = UoW;
            _commentRepository = (Repository<Comment>)_UoW.GetRepository<Comment>();
            _postRepository = (Repository<Post>)_UoW.GetRepository<Post>();
            _userRepository = (UserRepository)_UoW.GetRepository<User>();
        }

        public async Task<EntityBaseResponse<CommentDomain>> Get(int id)
        {
            Comment comment = await _commentRepository.Get(id);

            if (comment != null)
            {
                await _commentRepository.LoadNavigateProperty(comment);
                return new EntityBaseResponse<CommentDomain>(Helper.Mapper.Map<CommentDomain>(comment));
            }
            else
            {
                return new EntityBaseResponse<CommentDomain>($"Комментарий с Id = {id} не найден.");
            }
        }

        public async Task<EntityBaseResponse<CommentDomain>> Add(CommentDomain commentDomain)
        {
            Comment newComment = Helper.Mapper.Map<Comment>(commentDomain);
            newComment.DateAdd = DateTime.Now;
            await _commentRepository.Create(newComment);

            return new EntityBaseResponse<CommentDomain>(Helper.Mapper.Map<CommentDomain>(newComment));
        }


        public async Task<EntityBaseResponse<CommentDomain>> Update(CommentDomain commentDomain)
        {
            Comment comment = await _commentRepository.Get(commentDomain.Id);

            if (comment != null)
            {
                
                await _commentRepository.LoadNavigateProperty(comment);
                comment = Helper.Mapper.Map(commentDomain, comment);
                comment.User = _userRepository.Get(comment.UserId).Result;
                comment.Post = _postRepository.Get(comment.PostId).Result;
                await _commentRepository.Update(comment);
                
                return new EntityBaseResponse<CommentDomain>(Helper.Mapper.Map<CommentDomain>(comment));
            }
            else
            {
                return new EntityBaseResponse<CommentDomain>(false, $"Комментарий (Id = {commentDomain.Id}) не найден.");
            }
        }


        public async Task<EntityBaseResponse<CommentDomain>> Delete(CommentDomain commentDomain)
        {
            Comment comment = await _commentRepository.Get(commentDomain.Id);

            if (comment != null)
            {
                await _commentRepository.Delete(comment);
                return new EntityBaseResponse<CommentDomain>(true, $"Комментарий удален (Id = {comment.Id})", commentDomain);
            }
            else
            {
                return new EntityBaseResponse<CommentDomain>(false, $"Комментарий (Id = {commentDomain.Id}) не найден.");
            }
        }

        public async Task<EntityBaseResponse<IEnumerable<CommentDomain>>> GetByPost(int postId)
        {
            var commentList = await _commentRepository.Get(p => p.PostId == postId, c => c.OrderBy(c => c.DateAdd));
            return new EntityBaseResponse<IEnumerable<CommentDomain>>(Helper.Mapper.Map<IEnumerable<CommentDomain>>(commentList));
        }


        public EntityBaseResponse<IEnumerable<CommentDomain>> GetAll()
        {
            var commentList = _commentRepository.GetAll(p => p.User, p => p.Post);
            return new EntityBaseResponse<IEnumerable<CommentDomain>>(Helper.Mapper.Map<IEnumerable<CommentDomain>>(commentList));
        }
    }
}
