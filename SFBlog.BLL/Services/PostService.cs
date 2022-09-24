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
    public class PostService : IPostService
    {
        private IUnitOfWork _UoW;
        private Repository<Post> _postRepository;
        private Repository<Tag> _tagRepository;

        public PostService(IUnitOfWork UoW)
        {
            _UoW = UoW;
            _postRepository = (Repository<Post>)_UoW.GetRepository<Post>();
            _tagRepository = (Repository<Tag>)_UoW.GetRepository<Tag>();
        }

        public async Task<EntityBaseResponse<PostDomain>> Get(int id)
        {
            Post post = await _postRepository.Get(id);

            if (post != null)
            {
                post.ViewCount += 1;
                await _postRepository.Update(post);
                await _postRepository.LoadNavigateProperty(post);
                return new EntityBaseResponse<PostDomain>(Helper.Mapper.Map<PostDomain>(post));
            }
            else
            {
                return new EntityBaseResponse<PostDomain>($"Статья с Id = {id} не найдена.");
            }
        }

        public async Task<EntityBaseResponse<PostDomain>> Add(PostDomain postDomain)
        {
            Post newPost = Helper.Mapper.Map<Post>(postDomain);
            newPost.DateAdd = DateTime.Now;
            await _postRepository.Create(newPost);

            return new EntityBaseResponse<PostDomain>(Helper.Mapper.Map<PostDomain>(newPost));
        }


        public async Task<EntityBaseResponse<PostDomain>> Update(PostDomain postDomain)
        {
            Post post = await _postRepository.Get(postDomain.Id);

            if (post != null)
            {
                await _postRepository.LoadNavigateProperty(post);
                postDomain.UserId = post.UserId;
                postDomain.User = (Helper.Mapper.Map<UserDomain>(post.User));
                
                post = Helper.Mapper.Map(postDomain, post);
                await _postRepository.Update(post);
                
                return new EntityBaseResponse<PostDomain>(Helper.Mapper.Map<PostDomain>(post));
            }
            else
            {
                return new EntityBaseResponse<PostDomain>(false, $"Статья {postDomain.Title} (Id = {postDomain.Id}) не найдена.");
            }
        }


        public async Task<EntityBaseResponse<PostDomain>> Delete(PostDomain postDomain)
        {
            Post post = await _postRepository.Get(postDomain.Id);

            if (post != null)
            {
                await _postRepository.Delete(post);
                return new EntityBaseResponse<PostDomain>(true, $"Статья удалена {post.Title} (Id = {post.Id})", postDomain);
            }
            else
            {
                return new EntityBaseResponse<PostDomain>(false, $"Статья {postDomain.Title} (Id = {postDomain.Id}) не найдена.");
            }
        }


        public EntityBaseResponse<IEnumerable<PostDomain>> GetAll()
        {
            var postList = _postRepository.GetAll(p => p.User, p => p.PostTags, p => p.Comments);
            return new EntityBaseResponse<IEnumerable<PostDomain>>(Helper.Mapper.Map<IEnumerable<PostDomain>>(postList));
        }

        public IEnumerable<TagDomain> GetAllTags()
        {
            var tagList = _tagRepository.GetAll();
            return Helper.Mapper.Map<IEnumerable<TagDomain>>(tagList);
        }
    }
}
