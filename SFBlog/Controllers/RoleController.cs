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
    //[Route("Role")]
    public class RoleController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Role> _roleRepository;

        public RoleController(IMapper mapper, IUnitOfWork UoW)
        {
            _mapper = mapper;
            _UoW = UoW;
            _roleRepository = (Repository<Role>)_UoW.GetRepository<Role>();
        }

        [Route("AddRole")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View(); 
        }

        [Route("AddRole")]
        [HttpPost]
        public async Task<string> AddRole(RoleEditViewModel newRole, int userId)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<Role>(newRole);


                await _roleRepository.Create(role);
                return "Успех!";
            }
            return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            Role role = await _roleRepository.Get(id);
            RoleEditViewModel roleEdit = _mapper.Map<RoleEditViewModel>(role);

            return View(roleEdit);
        }

        [Authorize]
        [Route("EditRole")]
        [HttpPut]
        public async Task<string> Update(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleRepository.Get(model.Id);

                await _roleRepository.Update(role);

                return "Успех!";
            }
            else
            {
                return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
            }
        }


        [Authorize]
        [Route("DeleteRole")]
        [HttpDelete]
        public async Task<string> Delete(int roleId)
        {
            Role role = await _roleRepository.Get(roleId);
            if (role is null)
            {
                return $"Тэг с Id = {roleId} не найден";
            }

            await _roleRepository.Delete(role);
            return "Роль удалена.";
        }


        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var roleList = await Task.FromResult(_roleRepository.GetAll());
            List<RoleViewModel> resultRoleList = _mapper.Map<List<RoleViewModel>>(roleList);

            return View(resultRoleList);
        }

        [Authorize]
        [HttpGet]
        [Route("Role")]
        public async Task<RoleViewModel> GetRole(int roleId)
        {
            RoleViewModel resultRole = new RoleViewModel();

            Role role = await _roleRepository.Get(roleId);

            return _mapper.Map<RoleViewModel>(role);
        }
    }
}
