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
    public class RoleController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Role> _roleRepository;

        public RoleController(IMapper mapper, IUnitOfWork UoW, ILogger<UserController> logger)
        {
            _mapper = mapper;
            _UoW = UoW;
            _roleRepository = (Repository<Role>)_UoW.GetRepository<Role>();
            _logger = logger;
        }

        /// <summary>
        /// Запрос View добовления роли
        /// </summary>
        /// <returns>View добовления роли</returns>
        [Authorize(Roles="Администратор")]
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
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleEditViewModel newRole)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<Role>(newRole);


                await _roleRepository.Create(role);
                _logger.LogInformation($"Пользователь {User.Identity.Name} добавил роль {role.Description}.");
                return View("RoleList");
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
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            Role role = await _roleRepository.Get(id);
            RoleEditViewModel roleEdit = _mapper.Map<RoleEditViewModel>(role);

            return View(roleEdit);
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        /// <param name="model">ViewModel редактирования роли</param>
        /// <returns></returns>
        [Authorize(Roles = "Администратор")]
        [HttpPut]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleRepository.Get(model.Id);

                await _roleRepository.Update(role);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал роль id = {role.Id} {role.Description}");

                return View("RoleList");
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
        [Authorize(Roles = "Администратор")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int roleId)
        {
            Role role = await _roleRepository.Get(roleId);
            if (role is null)
            {
                return View("NotFound");
            }

            await _roleRepository.Delete(role);
            _logger.LogInformation($"Пользователь {User.Identity.Name} удалил роль {role.Description}.");
            return RedirectToAction("RoleList");
        }


        /// <summary>
        /// Получить View список ролей
        /// </summary>
        /// <returns>View списка ролей</returns>
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var roleList = await Task.FromResult(_roleRepository.GetAll());
            List<RoleViewModel> resultRoleList = _mapper.Map<List<RoleViewModel>>(roleList);

            return View(resultRoleList);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public async Task<IActionResult> ViewRole(int id)
        {

            Role role = await _roleRepository.Get(id);
            if (role == null)
            {
                _logger.LogInformation($"Роль с Id = {id} не найдена.");
                return View("NotFound");
            }

            return View(_mapper.Map<RoleViewModel>(role));
        }
    }
}
