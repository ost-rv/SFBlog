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
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Получить список ролей
        /// </summary>
        /// <returns>EntityBaseResponse<IEnumerable<RoleDomain>> результат</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<IEnumerable<RoleDomain>>> RoleList()
        {
            var result = await Task.FromResult(_roleService.GetAll());
            return result;
        }

        /// <summary>
        /// Добавление роли
        /// </summary>
        /// <param name="newRole">RoleDomain - новая роль</param>
        /// <returns>EntityBaseResponse<RoleDomain> результат добавления тэга</returns>
        [HttpPost("[action]")]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<EntityBaseResponse<RoleDomain>> AddRole([FromBody] RoleDomain newRole)
        {
            if (ModelState.IsValid)
            {
                EntityBaseResponse<RoleDomain> result = await _roleService.Add(newRole);
                return result;
            }
            else
            {
                return new EntityBaseResponse<RoleDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        /// <param name="model">RoleDomain редактируемая роль</param>
        /// <returns>EntityBaseResponse<RoleDomain> результат редактирования</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpPut("[action]/{id}")]
        public async Task<EntityBaseResponse<RoleDomain>> EditRole(int id, [FromBody] RoleDomain model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.Update(model);
                return result;
            }
            else
            {
                return new EntityBaseResponse<RoleDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>View списка статей</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpDelete("[action]/{id}")]
        public async Task<EntityBaseResponse<RoleDomain>> DeleteRole(int id)
        {

            EntityBaseResponse<RoleDomain> roleResponse = await _roleService.Get(id);
            if (!roleResponse.Success)
            {
                return roleResponse;
            }
            roleResponse = await _roleService.Delete(roleResponse.Entity);
            return roleResponse;
        }

        /// <summary>
        /// Получить роль
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>RoleDomain - модель роли/returns>>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<RoleDomain>> GetRole(int id)
        {
            var userResponse = await _roleService.Get(id);
            return userResponse;
        }
    }
}
