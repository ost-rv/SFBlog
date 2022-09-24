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
using SFBlog.Extensions;
using SFBlog.BLL.Services;
using SFBlog.BLL.Models;
using SFBlog.BLL.Response;

namespace SFBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private IMapper _mapper;
        private IPostService _postService;

        public PostController(IMapper mapper, IPostService postService, ILogger<PostController> logger)
        {
            _logger = logger; 
            _mapper = mapper;
            _postService = postService;
        }

        /// <summary>
        /// Запрос предсталвения добавления поста
        /// </summary>
        /// <returns>View добавления поста</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddPost()
        {
            PostEditViewModel post = new PostEditViewModel();
            var tags = await Task.FromResult(_postService.GetAllTags());
            post.CheckTags = tags.Select(t => new CheckTagViewModel { Id = t.Id, Designation = t.Designation, Checked = false }).ToList();

            return View(post);
        }

        /// <summary>
        /// Добавление поста
        /// </summary>
        /// <param name="newPost">ViewModel поста</param>
        /// <returns>View просмотра поста или в случае неудачного добавление view добавления поста</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPost(PostEditViewModel newPost)
        {
            if (ModelState.IsValid)
            {
                var post = _mapper.Map<PostDomain>(newPost);

                post.UserId = User.Identity.GeUsertId();

                EntityBaseResponse<PostDomain> result = await _postService.Add(post);

                if (result.Success)
                {
                    _logger.LogInformation($"Пользователь {User.Identity.Name} добавил пост {post.Title}.");
                    return RedirectToAction("ViewPost", new { result.Entity.Id });
                }
                else 
                {
                    View(newPost);
                }
                
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(newPost);
        }

        /// <summary>
        /// Получение View редактирования поста
        /// </summary>
        /// <param name="id">Идентификатор редактируемого поста</param>
        /// <returns>View редактирования поста</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditPost(int id)
        {
            EntityBaseResponse<PostDomain> post = await _postService.Get(id);
            if (post.Success)
            {
                List<TagDomain> allTags = _postService.GetAllTags().ToList();
                PostEditViewModel postEdit = _mapper.Map<PostEditViewModel>(post.Entity);

                postEdit.CheckTags = allTags.Select(t => new CheckTagViewModel
                {
                    Id = t.Id,
                    Designation = t.Designation,
                    Checked = post.Entity.Tags.Any(pt => pt.Id == t.Id)
                }).ToList();

                return View(postEdit);
            }
            else
            {
                return View("NotFound");
            }
        }


        /// <summary>
        /// Редактирование поста
        /// </summary>
        /// <param name="model">ViewModel редактирования поста</param>
        /// <returns>View поста или View редактирования поста</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPost(int id, PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = _mapper.Map<PostDomain>(model);
                await _postService.Update(post);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал статью {post.Title}");

                return RedirectToAction("ViewPost", new { id });
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(model);

        }

        /// <summary>
        /// Удалить пост
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <returns>View списка статей</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            EntityBaseResponse<PostDomain> post = await _postService.Get(id);
            if (!post.Success)
            {
                return View("NotFound");
            }

            post = await _postService.Delete(post.Entity);
            if (!post.Success)
            {
                _logger.LogInformation($"Пользователь {User.Identity.Name} удалил статью {post.Entity.Title}.");
            }
            
            return RedirectToAction("PostList");
        }

        /// <summary>
        /// Получить список постов
        /// </summary>
        /// <returns>View списка постов</returns>
        [HttpGet]
        public async Task<IActionResult> PostList()
        {
            var postList = await Task.FromResult(_postService.GetAll());
            List<PostViewModel> resultPostList = _mapper.Map<List<PostViewModel>>(postList.Entity);

            return View(resultPostList);
        }

        /// <summary>
        /// Получить пост по id
        /// </summary>
        /// <param name="id">идентификатор поста</param>
        /// <returns>View поста</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewPost(int id)
        {
            var postResponse = await _postService.Get(id);

            if (!postResponse.Success)
            {
                _logger.LogInformation(postResponse.Message);
                
                return View("NotFound");
            }

            return View(_mapper.Map<PostViewModel>(postResponse.Entity));

        }
    }
}
