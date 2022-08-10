using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using SFBlog.DAL.UoW;
using SFBlog.DAL.Models;
using SFBlog.DAL.Repository;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SFBlog.Models;

namespace SFBlog.Controllers
{
    public class PostController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Post> _postRepository;

        public PostController(IMapper mapper, IUnitOfWork UoW)
        {
            _mapper = mapper;
            _UoW = UoW;
            _postRepository = (Repository<Post>)_UoW.GetRepository<Post>();
        }

        [Route("AddPost")]
        [HttpPost]
        public async Task<string> Add(PostEditViewModel newPost, int userId)
        {
            if (ModelState.IsValid)
            {
                var post = _mapper.Map<Post>(newPost);

                post.DateAdd = DateTime.Now;
                post.UserId = userId;

                await _postRepository.Create(post);
                return "Успех!";
            }
            return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
        }


        [Authorize]
        [Route("EditPost")]
        [HttpPut]
        public async Task<string> Update(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = await _postRepository.Get(model.Id);

                await _postRepository.Update(post);

                return "Успех!";
            }
            else
            {
                return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
            }
        }


        [Authorize]
        [Route("DeletePost")]
        [HttpDelete]
        public async Task<string> Delete(int postId)
        {
            Post post = await _postRepository.Get(postId);
            if (post is null)
            {
                return $"Пост с Id = {postId} не найден";
            }

            await _postRepository.Delete(post);
            return "Пост удален.";
        }

        [Authorize]
        [HttpGet]
        [Route("PostList")]
        public List<PostViewModel> GetPostList()
        {
            List<PostViewModel> resultPostList = new List<PostViewModel>();

            var postList = _postRepository.GetAll();

            foreach (Post post in postList)
            {
                resultPostList.Add(_mapper.Map<PostViewModel>(post));
            }

            return resultPostList;
        }

        [Authorize]
        [HttpGet]
        [Route("Post")]
        public async Task<PostViewModel> GetPost(int postId)
        {
            PostViewModel resultPost = new PostViewModel();

            Post post = await _postRepository.Get(postId);

            return _mapper.Map<PostViewModel>(post);
        }
    }
}
