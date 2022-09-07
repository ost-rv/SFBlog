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

namespace SFBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Post> _postRepository;
        private Repository<Tag> _tagRepository;

        public PostController(IMapper mapper, IUnitOfWork UoW, ILogger<UserController> logger)
        {
            _logger = logger; ;
            _mapper = mapper;
            _UoW = UoW;
            _postRepository = (Repository<Post>)_UoW.GetRepository<Post>();
            _tagRepository = (Repository<Tag>)_UoW.GetRepository<Tag>();
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
            var tags = await Task.FromResult(_tagRepository.GetAll());
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
                var post = _mapper.Map<Post>(newPost);

                post.DateAdd = DateTime.Now;
                post.UserId = User.Identity.GeUsertId();

                await _postRepository.Create(post);
                _logger.LogInformation($"Пользователь {User.Identity.Name} добавил пост {post.Title}.");
                return RedirectToAction("ViewPost", new { id = post.Id } );
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


        /// <summary>
        /// Редактирование поста
        /// </summary>
        /// <param name="model">ViewModel редактирования поста</param>
        /// <returns>View поста или View редактирования поста</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditPost(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = await _postRepository.Get(model.Id);

                await _postRepository.Update(post);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал пост {post.Title}");

                return RedirectToAction("ViewPost", post.Id);
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
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            Post post = await _postRepository.Get(id);
            if (post is null)
            {
                ViewBag.Message = $"Пост с Id = {id} не найден";
                return View("PostList");
            }

            await _postRepository.Delete(post);
            _logger.LogInformation($"Пользователь {User.Identity.Name} удалил пост {post.Title}.");

            return View("PostList");
        }

        /// <summary>
        /// Получить список постов
        /// </summary>
        /// <returns>View списка постов</returns>
        [HttpGet]
        public async Task<IActionResult> PostList()
        {
            var postList = await Task.FromResult(_postRepository.GetAll());
            List<PostViewModel> resultPostList = _mapper.Map<List<PostViewModel>>(postList);

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
            Post post = (await Task.FromResult(_postRepository.Get(p => p.Id == id, 
                null, 
                "PostTags", 
                "User", 
                "Comments"))).Result.FirstOrDefault();

            if (post == null)
            {
                _logger.LogInformation($"Пост с Id = {id} не найден.");
                return View("NotFound");
            }

            PostViewModel postView = _mapper.Map<PostViewModel>(post);
            return View(postView);
        }
    }
}
