using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFBlog.API.Extensions;
using SFBlog.BLL.Models;
using SFBlog.BLL.Response;
using SFBlog.BLL.Services;
using SFBlog.DAL.UoW;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IUnitOfWork _UoW;
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Аунтификация пользователя
        /// </summary>
        /// <param name="model">модель аунтификации</param>
        /// <returns>EntityBaseResponse<UserDomain></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<EntityBaseResponse<UserDomain>> Authenticate([FromBody] UserAuthenticateDomain model)
        {
            EntityBaseResponse<UserDomain> userResponse = null;
            if (ModelState.IsValid)
            {

                userResponse = _userService.GetByLogin(model.Login);

                if (userResponse.Success)
                {
                    UserDomain userDomain = userResponse.Entity;
                    await Authenticate(userDomain); // аутентификация
                    userResponse.Message = "Успешная аунтификация";
                }
            }
            else
            {
                userResponse = new EntityBaseResponse<UserDomain>("Ошибки модели", ModelState.GetErrorMessages());
            }
            return userResponse;
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

        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="newUser">модель UserDomain нового пользователя</param>
        /// <returns>Task<EntityBaseResponse<UserDomain>> результат регистрации</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<EntityBaseResponse<UserDomain>> Register([FromBody] UserDomain newUser)
        {
            if (ModelState.IsValid)
            {
                // добавляем пользователя в бд
                var resultRegister = await _userService.Register(newUser);
                // аутентификация
                await Authenticate(resultRegister.Entity);
                return resultRegister;
            }
            else
            {
                return new EntityBaseResponse<UserDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <returns>View аунтификации</returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<BaseResponse> Logout()
        {
            string message =($"Пользователь {User.Identity.Name} вышел");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new EntityBaseResponse<UserDomain>(true, message);
        }


        /// <summary>
        /// Редактирования пользователя
        /// </summary>
        /// <param name="model">модель UserDomain редактируемого пользователя</param>
        /// <returns>Task<EntityBaseResponse<UserDomain>> результат</returns>
        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<EntityBaseResponse<UserDomain>> EditUser(int id, [FromBody] UserDomain model)
        {
            if (ModelState.IsValid)
            {
                EntityBaseResponse<UserDomain> userResponse = await _userService.Update(model);
                return userResponse;
            }
            else
            {
                return new EntityBaseResponse<UserDomain>("Ошибки модели.", ModelState.GetErrorMessages());
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BaseResponse результат удаления</returns>
        [HttpDelete("[action]/{id}")]
        [Authorize(Roles = "Aдминистратор")]
        public async Task<BaseResponse> DeleteUser(int id)
        {
            EntityBaseResponse<UserDomain> userResponse = await _userService.Get(id);
            if (!userResponse.Success)
            {
                return userResponse;
            }
            userResponse = await _userService.Delete(userResponse.Entity);
            return userResponse;

        }
        /// <summary>
        /// Получение списка пользоватей
        /// </summary>
        /// <param name="id"></param>
        /// <returns>EntityBaseResponse<IEnumerable<UserDomain>> результат</returns>
        [HttpGet("[action]")]
        //[Authorize(Roles = "Aдминистратор")]
        public async Task<EntityBaseResponse<IEnumerable<UserDomain>>> UserList()
        {
            var userList = await Task.FromResult(_userService.GetAll());

            return userList;
        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>UserDomain - модель пользователя</returns>
        [Authorize(Roles = "Aдминистратор")]
        [HttpGet("[action]")]
        public async Task<EntityBaseResponse<UserDomain>> GetUser(int id)
        {
            var userResponse = await _userService.Get(id);
            return userResponse;
        }
    }
}
