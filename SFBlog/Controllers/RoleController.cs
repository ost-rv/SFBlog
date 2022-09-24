using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using SFBlog.DAL.Models;
using SFBlog.Models;
using SFBlog.Extensions;
using SFBlog.BLL.Services;
using SFBlog.BLL.Models;
using SFBlog.BLL.Response;

namespace SFBlog.Controllers
{
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        private IMapper _mapper;
        private IRoleService _roleService;

        public RoleController(IMapper mapper, IRoleService roleService, ILogger<RoleController> logger)
        {
            _mapper = mapper;
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Запрос View добовления роли
        /// </summary>
        /// <returns>View добовления роли</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View(); 
        }
        /// <summary>
        /// Добовление роли
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns>View список ролей</returns>
        [HttpPost]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> AddRole(RoleEditViewModel newRole)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<RoleDomain>(newRole);

                EntityBaseResponse<RoleDomain> result = await _roleService.Add(role);
                if (result.Success)
                {
                    _logger.LogInformation($"Пользователь {User.Identity.Name} добавил роль {role.Description}.");
                    return RedirectToAction("RoleList");
                }
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(newRole);
        }

        /// <summary>
        /// Запрос View для редактирования роли
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>View для редактирования роли</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            EntityBaseResponse<RoleDomain> role = await _roleService.Get(id);
            if (role.Success)
            {
                RoleEditViewModel roleEdit = _mapper.Map<RoleEditViewModel>(role.Entity);
                return View(roleEdit);
            }
            else
            {
                return View("NotFound");
            }
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        /// <param name="model">ViewModel редактирования роли</param>
        /// <returns></returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<RoleDomain>(model);
                await _roleService.Update(role);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал роль id = {role.Id} {role.Description}");

                return RedirectToAction("RoleList");
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(model);
        }

        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>View списка ролей</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            EntityBaseResponse<RoleDomain> role = await _roleService.Get(id);
            if (!role.Success)
            {
                return View("NotFound");
            }

            role = await _roleService.Delete(role.Entity);
            if (role.Success)
            {
                _logger.LogInformation($"Пользователь {User.Identity.Name} удалил роль {role.Entity.Description}.");
            }

            return RedirectToAction("RoleList");
        }


        /// <summary>
        /// Получить View список ролей
        /// </summary>
        /// <returns>View списка ролей</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var roleList = await Task.FromResult(_roleService.GetAll());
            List<RoleViewModel> resultRoleList = _mapper.Map<List<RoleViewModel>>(roleList.Entity);

            return View(resultRoleList);
        }

        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public async Task<IActionResult> ViewRole(int id)
        {

            var roleResponse = await _roleService.Get(id);
            if (!roleResponse.Success)
            {
                _logger.LogInformation($"Роль с Id = {id} не найдена.");
                return View("NotFound");
            }

            return View(_mapper.Map<RoleViewModel>(roleResponse.Entity));

        }
    }
}
