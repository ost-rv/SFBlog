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
using SFBlog.BLL.Models;
using SFBlog.BLL.Services;
using SFBlog.BLL.Response;

namespace SFBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private IUserService _userService;

        public UserController(ILogger<UserController> logger,
            IMapper mapper,
            IUserService userService)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
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

                EntityBaseResponse<UserDomain> userResponse = _userService.GetByLogin(model.Login);

                if (userResponse.Success)
                {

                    UserDomain userDomain = userResponse.Entity;
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
                var user = _mapper.Map<UserDomain>(newUser);

                // добавляем пользователя в бд
                var resultRegister = await _userService.Register(user);
                // аутентификация
                await Authenticate(resultRegister.Entity); 

                return RedirectToAction("PostList", "Post");
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
            var responseService = await _userService.Get(id);
            if (responseService.Success)
            {
                UserDomain user = responseService.Entity;
                List<RoleDomain> allRoles = _userService.GetAllRoles().ToList();

                UserEditViewModel userEdit = _mapper.Map<UserEditViewModel>(user);
                userEdit.CheckRoles = allRoles.Select(r => new CheckRoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Checked = user.Roles.Any(ur => ur.Id == r.Id)
                }).ToList();

                return View(userEdit);

            }
            else 
            {
                return View("NotFound");
            }
        }

        /// <summary>
        /// Редактирования пользователя
        /// </summary>
        /// <param name="model">ViewModel для редактирования пользователя</param>
        /// <returns>View редактирования пользователя</returns>
        //[Route("[controller]/[action]")]
        
        [HttpPost]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> EditUser(int id, UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<UserDomain>(model);
                await _userService.Update(user);
                _logger.LogInformation($"Редактирование пользователя {user.Login} пользователем {User.Identity.Name}");

                return RedirectToAction("UserView", new { id });
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
        /// <param name="id"></param>
        /// <returns>View список пользователя</returns>
        [HttpGet]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> Delete(int id, List<SFBlog.Models.UserViewModel> model)
        {
            EntityBaseResponse<UserDomain> userResponse = await _userService.Get(id);
            if (!userResponse.Success)
            {
                ViewBag.Message = userResponse.Message;
                return View("UserList", model);
            }
            userResponse = await _userService.Delete(userResponse.Entity);
            if (!userResponse.Success)
            {
                ViewBag.Message = userResponse.Message;
                return View("UserList", model);
            }

            _logger.LogInformation("Пользователь удален.");
            //ViewBag.Message = $"Пользователь {userResponse.Entity.Login} удален.";
            
            return RedirectToAction("UserList");
        }

        /// <summary>
        /// Запрос View список пользователей
        /// </summary>
        /// <returns>View список пользователей пользователя</returns>
        [HttpGet]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<IActionResult> UserList()
        {
            var userList = await Task.FromResult(_userService.GetAll());
            List<UserViewModel> resultUserList = _mapper.Map<List<UserViewModel>>(userList.Entity);
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
            var userResponse = await _userService.Get(id);
            if (!userResponse.Success )
            {
                _logger.LogInformation(userResponse.Message);
                ViewBag.Message = userResponse.Message;
                return View("NotFound");
            }
            return View(_mapper.Map<UserViewModel>(userResponse.Entity));
        }
    }
}
