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
using SFBlog.BLL.Models;
using SFBlog.Extensions;

namespace SFBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private UserRepository _userRepository;
        private IRepository<UserRole> _userRoleRepository;
        private IRepository<Role> _roleRepository;

        public UserController(ILogger<UserController> logger,
            IMapper mapper, 
            IUnitOfWork UoW)
        {
            _logger = logger;
            _mapper = mapper;
            _UoW = UoW;
            _userRepository = (UserRepository)_UoW.GetRepository<User>();
            _userRoleRepository = _UoW.GetRepository<UserRole>();
            _roleRepository = _UoW.GetRepository<Role>();
        }
        /// <summary>
        /// Запрос View для аунтификации
        /// </summary>
        /// <returns>View аунтификации</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Authenticate()
        {
            return View();
        }
        /// <summary>
        /// Аунтификация пользователя
        /// </summary>
        /// <param name="model">ViewModel пользованеля</param>
        /// <returns>В случае ошибок в заполнеии модел возращает обратно представление модели
        /// иначе предсталение списка статей</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate(UserAuthenticateViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userRepository.GetByLogin(model.Login);

                if (user != null && user.Password == model.Password)
                {
                    UserDomain userDomain = UserDomain.CreateUserDomain(user);
                    await Authenticate(userDomain); // аутентификация
                    return RedirectToAction("PostList", "Post");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                _logger.LogInformation(ModelState.GetAllError());
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(model);
        }

        private async Task Authenticate(UserDomain userDomain)
        {
            // создаем claim для логина и ролей
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userDomain.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userDomain.Roles.FirstOrDefault()?.Name),
                new Claim(ClaimTypes.NameIdentifier, userDomain.Id.ToString(), ClaimValueTypes.Integer),
            };
            
           // создаем объект ClaimsIdentity
           ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                "AppCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation($"Успешная аутентификация пользователя: {User.Identity.Name}") ;
        }

        /// <summary>
        /// Запрос View для регистрации
        /// </summary>
        /// <returns>View аунтификации</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Регистрация пользователя в приложении
        /// </summary>
        /// <param name="newUser">ViewModel нового пользователя</param>
        /// <returns>View список статей или при неудачной регистрации View регистрации</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(newUser);

                UserRole userRole = new UserRole { User = user, RoleId = 3 /*Id роли пользованель*/ };
                user.UserRoles.Add(userRole);

                // добавляем пользователя в бд
                await _userRepository.Create(user);
                await Authenticate(_mapper.Map<UserDomain>(user)); // аутентификация

                return View("/Post/PostList");
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }
            
            return View(newUser);
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <returns>View аунтификации</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"Пользователь {User.Identity.Name} вышел"); 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Authenticate");
        }

        /// <summary>
        /// Запрос View для редактирования пользователя
        /// </summary>
        /// <returns>View редактирования пользователя</returns>
        /// <param name="id">Идентификатор пользователя</param>
        [HttpGet]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> EditUser(int id)
        {
            User user = await _userRepository.Get(id);
            List<Role> allRoles = await Task.FromResult(_roleRepository.GetAll().ToList());
            UserEditViewModel userEdit = _mapper.Map<UserEditViewModel>(user);
            userEdit.CheckRoles = allRoles.Select(r => new CheckRoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Checked = user.UserRoles.Any(ur => ur.RoleId == r.Id)
            }).ToList();

            return View(userEdit);

        }

        /// <summary>
        /// Редактирования пользователя
        /// </summary>
        /// <param name="model">ViewModel для редактирования пользователя</param>
        /// <returns>View редактирования пользователя</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpPut]
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Get(model.Id);

                await _userRepository.Update(user);
                _logger.LogInformation($"Редактирование пользователя {user.Login} пользователем {User.Identity.Name}");

                return View(user.Id);
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }
            return View(model);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>View список пользователя</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            User user = await _userRepository.Get(userId);
            if (user is null)
            {
                ViewBag.Message =  $"Пользователь с Id = {userId} не найден";
                return View("UserList");
            }
            await _userRepository.Delete(user);
            _logger.LogInformation("Пользователь удален.");
            ViewBag.Message = $"Пользователь {user.Login} удален.";
            
            return View("UserList");
        }

        /// <summary>
        /// Запрос View список пользователей
        /// </summary>
        /// <returns>View список пользователей пользователя</returns>
        [HttpGet]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> UserList()
        {
            var userList = await Task.FromResult(_userRepository.GetAll());
            List<UserViewModel> resultUserList = _mapper.Map<List<UserViewModel>>(userList);
            _logger.LogInformation($"Пользователь {User.Identity.Name} запросил список пользователей.");

            return View(resultUserList);
        }

        /// <summary>
        /// Запрос View пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>View пользователя</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet]
        public async Task<IActionResult> UserView(int id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
            {
                _logger.LogInformation($"Пользователь с Id = {id} не найден.");
                ViewBag.Message = $"Пользователь с Id = {id} не найден.";
                UserViewModel resultUser = new UserViewModel();
                return View(resultUser);
            }
            return View(_mapper.Map<UserViewModel>(user));
        }
    }
}
