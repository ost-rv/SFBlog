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
    public class PostController : ControllerBase
    {
        private IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Получить список постов
        /// </summary>
        /// <returns>EntityBaseResponse<IEnumerable<PostDomain>> результат</returns>
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<IEnumerable<PostDomain>>> PostList()
        {
            var result = await Task.FromResult(_postService.GetAll());
            return result;
        }

        /// <summary>
        /// Получить пост
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <returns>PostDomain - модель поста/returns>>
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<PostDomain>> GetPost(int id)
        {
            var userResponse = await _postService.Get(id);
            return userResponse;

        }

        /// <summary>
        /// Добавление поста
        /// </summary>
        /// <param name="newPost">PostDomain - новый пост</param>
        /// <returns>EntityBaseResponse<PostDomain> результат добавления поста</returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<EntityBaseResponse<PostDomain>> AddPost([FromBody] PostDomain newPost)
        {
            if (ModelState.IsValid)
            {
                newPost.UserId = User.Identity.GeUsertId();

                EntityBaseResponse<PostDomain> result = await _postService.Add(newPost);
                return result;
            }
            else
            {
                return new EntityBaseResponse<PostDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Редактирование поста
        /// </summary>
        /// <param name="model">PostDomain редактируемый пост</param>
        /// <returns>EntityBaseResponse<PostDomain> результат редактирования</returns>
        [Authorize]
        [HttpPut("[action]/{id}")]
        public async Task<EntityBaseResponse<PostDomain>> EditPost(int id, [FromBody] PostDomain model)
        {
            if (ModelState.IsValid)
            {
                var result = await _postService.Update(model);

                return result;
            }
            else
            {
                return new EntityBaseResponse<PostDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }


        /// <summary>
        /// Удалить пост
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <returns>EntityBaseResponse<PostDomain> результат удаление</returns>
        [Authorize]
        [HttpDelete("[action]/{id}")]
        public async Task<EntityBaseResponse<PostDomain>> DeletePost(int id)
        {

            EntityBaseResponse<PostDomain> postResponse = await _postService.Get(id);
            if (!postResponse.Success)
            {
                return postResponse;
            }
            postResponse = await _postService.Delete(postResponse.Entity);
            return postResponse;
        }
    }
}
