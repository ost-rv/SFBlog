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
    public class TagController : ControllerBase
    {
        private ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Получить список тэгов
        /// </summary>
        /// <returns>EntityBaseResponse<IEnumerable<TagDomain>> результат</returns>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<IEnumerable<TagDomain>>> TagList()
        {
            var result = await Task.FromResult(_tagService.GetAll());
            return result;
        }

        /// <summary>
        /// Добавление тэга
        /// </summary>
        /// <param name="newTag">TagDomain - новый тэг</param>
        /// <returns>EntityBaseResponse<TagDomain> результат добавления тэга</returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<EntityBaseResponse<TagDomain>> AddTag([FromBody] TagDomain newTag)
        {
            if (ModelState.IsValid)
            {
                EntityBaseResponse<TagDomain> result = await _tagService.Add(newTag);
                return result;
            }
            else
            {
                return new EntityBaseResponse<TagDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Редактирование тэга
        /// </summary>
        /// <param name="model">TagDomain редактируемый тэг</param>
        /// <returns>EntityBaseResponse<TagDomain> результат редактирования</returns>
        [Authorize]
        [HttpPut("[action]/{id}")]
        public async Task<EntityBaseResponse<TagDomain>> EditTag(int id, [FromBody] TagDomain model)
        {
            if (ModelState.IsValid)
            {
                var result = await _tagService.Update(model);

                return result;
            }
            else
            {
                return new EntityBaseResponse<TagDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Удалить тэг
        /// </summary>
        /// <param name="id">Идентификатор тэга</param>
        /// <returns>EntityBaseResponse<TagDomain> результат удаления тэга</returns>
        [Authorize]
        [HttpDelete("[action]/{id}")]
        public async Task<EntityBaseResponse<TagDomain>> DeleteTag(int id)
        {
            EntityBaseResponse<TagDomain> tagResponse = await _tagService.Get(id);
            if (!tagResponse.Success)
            {
                return tagResponse;
            }
            tagResponse = await _tagService.Delete(tagResponse.Entity);
            return tagResponse;
        }

        /// <summary>
        /// Получить тэг
        /// </summary>
        /// <param name="id">Идентификатор тэга</param>
        /// <returns>TagDomain - модель тэга/returns>>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<TagDomain>> GetTag(int id)
        {
            var userResponse = await _tagService.Get(id);
            return userResponse;
        }
    }
}
