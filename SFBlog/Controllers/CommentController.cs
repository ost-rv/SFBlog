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
    public class CommentController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Comment> _commentRepository;

        public CommentController(IMapper mapper, IUnitOfWork UoW, ILogger<UserController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _UoW = UoW;
            _commentRepository = (Repository<Comment>)_UoW.GetRepository<Comment>();
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
                var comment = _mapper.Map<Comment>(newComment);

                comment.DateAdd = DateTime.Now;
                comment.UserId = User.Identity.GeUsertId();
               
                await _commentRepository.Create(comment);
                _logger.LogInformation($"Пользователь {User.Identity.Name} добавил комментарий.");
                return View();
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
        [HttpPut]
        public async Task<string> EditComment(CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = await _commentRepository.Get(model.Id);

                await _commentRepository.Update(comment);

                return "Успех!";
            }
            else
            {
                return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<string> Delete(int commentId)
        {
            Comment comment = await _commentRepository.Get(commentId);
            if (comment is null)
            {
                return $"Пост с Id = {commentId} не найден";
            }

            await _commentRepository.Delete(comment);
            _logger.LogInformation($"Пользователь {User.Identity.Name} удалил комментарий.");
            return "Пост удален.";


        }
        /// <summary>
        /// Список всех комментариев
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CommentList()
        {
            var commentList = await Task.FromResult(_commentRepository.GetAll());
            List<CommentViewModel> resultCommetList = _mapper.Map<List<CommentViewModel>>(commentList);

            return View(resultCommetList);
        }

        /// <summary>
        /// Список комментариев к постсу
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PostCommentList(int postId)
        {
            var commentList = await Task.FromResult(_commentRepository.Get(c => c.PostId == postId, c => c.OrderBy(c=>c.DateAdd)));
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
        public async Task<CommentViewModel> ViewComment(int commentId)
        {
            CommentViewModel resultComment = new CommentViewModel();

            Comment comment = await _commentRepository.Get(commentId);

            return _mapper.Map<CommentViewModel>(comment);
        }
    }
}
