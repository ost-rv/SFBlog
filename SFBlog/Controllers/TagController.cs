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
    public class TagController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Tag> _tagRepository;

        public TagController(IMapper mapper, IUnitOfWork UoW, ILogger<UserController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _UoW = UoW;
            _tagRepository = (Repository<Tag>)_UoW.GetRepository<Tag>();
        }

        /// <summary>
        /// Запрос View добовления тэга
        /// </summary>
        /// <returns>View добовления тэга</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Добовление тэга
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns>View список тэгов</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTag(TagEditViewModel newTag)
        {
            if (ModelState.IsValid)
            {
                var tag = _mapper.Map<Tag>(newTag);

                await _tagRepository.Create(tag);
                _logger.LogInformation($"Пользователь {User.Identity.Name} добавил тэг {tag.Designation}.");

                return RedirectToAction("TagList");
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(newTag);
        }

        /// <summary>
        /// Запрос View для редактирования тэга
        /// </summary>
        /// <param name="id">Идентификатор тэга</param>
        /// <returns>View для редактирования тэга</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTag(int id)
        {
            Tag tag = await _tagRepository.Get(id);
            TagEditViewModel tagEdit = _mapper.Map<TagEditViewModel>(tag);

            return View(tagEdit);
        }

        /// <summary>
        /// Редактирование тэга
        /// </summary>
        /// <param name="model">ViewModel редактирования тэга</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditTag(TagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = await _tagRepository.Get(model.Id);

                await _tagRepository.Update(tag);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал тэг id = {tag.Id} {tag.Designation}");

                return View("TagList");
            }
            else
            {
                _logger.LogInformation(ModelState.GetAllError());
            }

            return View(model);

        }
        /// <summary>
        /// Удалить тэг
        /// </summary>
        /// <param name="Id">Идентификатор тэга</param>
        /// <returns>View списка тэгов</returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int tagId)
        {
            Tag tag = await _tagRepository.Get(tagId);
            if (tag is null)
            {
                return View("NotFound");
            }

            await _tagRepository.Delete(tag); 
            _logger.LogInformation($"Пользователь {User.Identity.Name} удалил тэг {tag.Designation}.");
            return RedirectToAction("TagList");

        }

        /// <summary>
        /// Получить View список тэгов
        /// </summary>
        /// <returns>View списка тэгов</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TagList()
        {
            var tagList = await Task.FromResult(_tagRepository.GetAll());
            List<TagViewModel> resultTagList = _mapper.Map<List<TagViewModel>>(tagList);

            return View(resultTagList);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewTag(int id)
        {

            Tag tag = await _tagRepository.Get(id);
            if (tag == null)
            {
                _logger.LogInformation($"Пост с Id = {id} не найден.");
                return View("NotFound");
            }


            return View(_mapper.Map<TagViewModel>(tag));
        }
    }
}
