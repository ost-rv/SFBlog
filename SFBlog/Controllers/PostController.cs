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
        private Repository<Tag> _tagRepository;

        public PostController(IMapper mapper, IUnitOfWork UoW)
        {
            _mapper = mapper;
            _UoW = UoW;
            _postRepository = (Repository<Post>)_UoW.GetRepository<Post>();
            _tagRepository = (Repository<Tag>)_UoW.GetRepository<Tag>();
        }

        [HttpGet]
        public async Task<IActionResult> AddPost()
        {

            PostEditViewModel post = new PostEditViewModel();

            var tags = await Task.FromResult(_tagRepository.GetAll());

            post.CheckTags = tags.Select(t => new CheckTagViewModel { Id = t.Id, Designation = t.Designation, Checked = false }).ToList();

            return View(post);
        }

        [HttpPost]
        public async Task<string> AddPost(PostEditViewModel newPost)
        {
            if (ModelState.IsValid)
            {
                var post = _mapper.Map<Post>(newPost);

                post.DateAdd = DateTime.Now;
                //post.UserId = userId;

                await _postRepository.Create(post);
                return "Успех!";
            }
            return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            Post post = (await Task.FromResult(_postRepository.Get(p => p.Id == id, null, "PostTags"))).Result.FirstOrDefault();
            List<Tag> allTags = await Task.FromResult(_tagRepository.GetAll().ToList());
            PostEditViewModel postEdit = _mapper.Map<PostEditViewModel>(post);
            postEdit.CheckTags = allTags.Select(t => new CheckTagViewModel
            {
                Id = t.Id,
                Designation = t.Designation,
                Checked = post.PostTags.Any(pt => pt.TagId == t.Id)
            }).ToList();

            return View(postEdit);
        }

        //[Authorize]
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

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> PostList()
        {
            var postList = await Task.FromResult(_postRepository.GetAll());
            List<PostViewModel> resultPostList = _mapper.Map<List<PostViewModel>>(postList);

            return View(resultPostList);
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewPost(int id)
        {
            Post post = (await Task.FromResult(_postRepository.Get(p => p.Id == id, 
                null, 
                "PostTags", 
                "User", 
                "Comments"))).Result.FirstOrDefault();
            
            List<Tag> allTags = await Task.FromResult(_tagRepository.GetAll().ToList());
            PostViewModel postView = _mapper.Map<PostViewModel>(post);

            return View(postView);
        }
    }
}
