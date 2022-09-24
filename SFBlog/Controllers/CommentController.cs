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
    public class CommentController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private ICommentService _commentService;

        public CommentController(IMapper mapper, ICommentService commentService, ILogger<UserController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// Запрос представления для добавления комментария
        /// </summary>
        /// <param name="postId">Идентификатор поста к которому добавляется комментарий</param>
        /// <returns>View добавления комментария</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddComment(int postId)
        {
            CommentEditViewModel newComment = new CommentEditViewModel();
            newComment.PostId = postId;
            return PartialView(newComment);
        }

        /// <summary>
        /// Добавления комментария
        /// </summary>
        /// <param name="newComment">ViewModel комментария</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentEditViewModel newComment)
        {
            if (ModelState.IsValid)
            {
                var comment = _mapper.Map<CommentDomain>(newComment);
                comment.UserId = User.Identity.GeUsertId();
                EntityBaseResponse<CommentDomain> result = await _commentService.Add(comment);
                if (result.Success)
                {
                    _logger.LogInformation($"Пользователь {User.Identity.Name} добавил комментарий.");
                    return RedirectToAction("ViewPost", "Post", new { id = comment.PostId } );
                }
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }
            return View(newComment);
        }

        /// <summary>
        /// Редактирования комментария
        /// </summary>
        /// <param name="model">ViewModel редактирования</param>
        /// <returns>строка результа</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditComment(CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = _mapper.Map<CommentDomain>(model);

                await _commentService.Update(comment);

                return RedirectToAction("PostView", "Post", new { id = model.PostId } );
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            EntityBaseResponse<CommentDomain> comment = await _commentService.Get(id);

            if (!comment.Success)
            {
                return View("NotFound");
            }

            comment = await _commentService.Delete(comment.Entity);
            if (comment.Success)
            {
                _logger.LogInformation($"Пользователь {User.Identity.Name} удалил комментарий.");

            }
            return RedirectToAction("CommentList");

        }
        /// <summary>
        /// Список всех комментариев
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CommentList()
        {
            var commentList = await Task.FromResult(_commentService.GetAll());
            List<CommentViewModel> resultCommetList = _mapper.Map<List<CommentViewModel>>(commentList.Entity);

            return View(resultCommetList);
        }

        /// <summary>
        /// Список комментариев к постсу
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PostCommentList(int postId)
        {
            var commentList = await Task.FromResult(_commentService.GetByPost(postId));
            List<CommentViewModel> resultCommetList = _mapper.Map<List<CommentViewModel>>(commentList);

            return View(resultCommetList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewComment(int id)
        {
            var commentResponse = await _commentService.Get(id);

            if (!commentResponse.Success)
            {
                _logger.LogInformation(commentResponse.Message);
                return View("NotFound");
            }

            return View(_mapper.Map<TagViewModel>(commentResponse.Entity));
        }
    }
}
