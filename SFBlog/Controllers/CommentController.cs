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
    public class CommentController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Comment> _commentRepository;

        public CommentController(IMapper mapper, IUnitOfWork UoW)
        {
            _mapper = mapper;
            _UoW = UoW;
            _commentRepository = (Repository<Comment>)_UoW.GetRepository<Comment>();
        }

        [Route("AddComment")]
        [HttpPost]
        public async Task<string> Add(CommentEditViewModel newComment, int userId)
        {
            if (ModelState.IsValid)
            {
                var comment = _mapper.Map<Comment>(newComment);

                comment.DateAdd = DateTime.Now;
                comment.UserId = userId;

                await _commentRepository.Create(comment);
                return "Успех!";
            }
            return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
        }


        [Authorize]
        [Route("EditComment")]
        [HttpPut]
        public async Task<string> Update(CommentEditViewModel model)
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
        [Route("DeleteComment")]
        [HttpDelete]
        public async Task<string> Delete(int commentId)
        {
            Comment comment = await _commentRepository.Get(commentId);
            if (comment is null)
            {
                return $"Пост с Id = {commentId} не найден";
            }

            await _commentRepository.Delete(comment);
            return "Пост удален.";
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> CommentList()
        {
            var commentList = await Task.FromResult(_commentRepository.GetAll());
            List<CommentViewModel> resultCommetList = _mapper.Map<List<CommentViewModel>>(commentList);

            return View(resultCommetList);
        }

        [Authorize]
        [HttpGet]
        public async Task<CommentViewModel> GetComment(int commentId)
        {
            CommentViewModel resultComment = new CommentViewModel();

            Comment comment = await _commentRepository.Get(commentId);

            return _mapper.Map<CommentViewModel>(comment);
        }
    }
}
