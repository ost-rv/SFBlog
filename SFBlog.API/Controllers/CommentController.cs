using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFBlog.API.Extensions;
using SFBlog.BLL.Models;
using SFBlog.BLL.Response;
using SFBlog.BLL.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Получить список комментариев
        /// </summary>
        /// <returns>EntityBaseResponse<IEnumerable<CommentDomain>> результат</returns>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<IEnumerable<CommentDomain>>> CommentList()
        {
            var result = await Task.FromResult(_commentService.GetAll());
            return result;
        }

        /// <summary>
        /// Получить список комментариев к посту
        /// </summary>
        /// <returns>EntityBaseResponse<IEnumerable<CommentDomain>> результат</returns>
        [Authorize]
        [HttpGet("[action]/{postId}")]
        public async Task<EntityBaseResponse<IEnumerable<CommentDomain>>> GetByPost(int postId)
        {
            var result = await _commentService.GetByPost(postId);
            return result;
        }

        /// <summary>
        /// Добавление комментария
        /// </summary>
        /// <param name="newComment">CommentDomain - новый комментарий</param>
        /// <returns>EntityBaseResponse<CommentDomain> результат добавления комментария</returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<EntityBaseResponse<CommentDomain>> AddComment([FromBody] CommentDomain newComment)
        {
            if (ModelState.IsValid)
            {
                EntityBaseResponse<CommentDomain> result = await _commentService.Add(newComment);
                return result;
            }
            else
            {
                return new EntityBaseResponse<CommentDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        /// <param name="model">CommentDomain редактируемый комментарий</param>
        /// <returns>EntityBaseResponse<CommentDomain> результат редактирования</returns>
        [Authorize]
        [HttpPut("[action]/{id}")]
        public async Task<EntityBaseResponse<CommentDomain>> EditComment(int id, [FromBody] CommentDomain model)
        {
            if (ModelState.IsValid)
            {
                var result = await _commentService.Update(model);

                return result;
            }
            else
            {
                return new EntityBaseResponse<CommentDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Удалить комментарий
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        /// <returns>EntityBaseResponse<CommentDomain> результат удаления комментария</returns>
        [Authorize]
        [HttpDelete("[action]/{id}")]
        public async Task<EntityBaseResponse<CommentDomain>> DeleteComment(int id)
        {
            EntityBaseResponse<CommentDomain> commentResponse = await _commentService.Get(id);
            if (!commentResponse.Success)
            {
                return commentResponse;
            }
            commentResponse = await _commentService.Delete(commentResponse.Entity);
            return commentResponse;
        }

        /// <summary>
        /// Получить комментарий
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        /// <returns>CommentDomain - модель комментария/returns>>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<CommentDomain>> GetComment(int id)
        {
            var userResponse = await _commentService.Get(id);
            return userResponse;
        }
    }
}
