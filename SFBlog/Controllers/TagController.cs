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
using SFBlog.BLL.Services;
using SFBlog.BLL.Models;
using SFBlog.BLL.Response;

namespace SFBlog.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;
        private IMapper _mapper;
        private ITagService _tagService;

        public TagController(IMapper mapper, ITagService tagService, ILogger<TagController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _tagService = tagService;
        }

        /// <summary>
        /// Запрос View добовления тэга
        /// </summary>
        /// <returns>View добовления тэга</returns>
        [Authorize]
        [HttpGet]
        public IActionResult AddTag()
        {
            return View();
        }

        /// <summary>
        /// Добовление тэга
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns>View список тэгов</\eturns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTag(TagEditViewModel newTag)
        {
            if (ModelState.IsValid)
            {
                var tag = _mapper.Map<TagDomain>(newTag);

                EntityBaseResponse<TagDomain> result  = await _tagService.Add(tag);
                if (result.Success)
                {
                    _logger.LogInformation($"Пользователь {User.Identity.Name} добавил тэг {tag.Designation}.");
                    return RedirectToAction("TagList");
                }
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
            EntityBaseResponse<TagDomain> tag = await _tagService.Get(id);
            if (tag.Success)
            {
                TagEditViewModel tagEdit = _mapper.Map<TagEditViewModel>(tag.Entity);
                return View(tagEdit);
            }
            else
            {
                return View("NotFound");
            }
        }

        /// <summary>
        /// Редактирование тэга
        /// </summary>
        /// <param name="model">ViewModel редактирования тэга</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditTag(TagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = _mapper.Map<TagDomain>(model);
                await _tagService.Update(tag);
                _logger.LogInformation($"Пользователь {User.Identity.Name} отредактировал тэг id = {tag.Id} {tag.Designation}");

                return RedirectToAction("TagList");
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
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            EntityBaseResponse<TagDomain> tag = await _tagService.Get(id);
            if (!tag.Success)
            {
                return View("NotFound");
            }

            tag = await _tagService.Delete(tag.Entity);
            if (tag.Success)
            {
                _logger.LogInformation($"Пользователь {User.Identity.Name} удалил тэг {tag.Entity.Designation}.");
                
            }
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
            var tagList = await Task.FromResult(_tagService.GetAll());
            List<TagViewModel> resultTagList = _mapper.Map<List<TagViewModel>>(tagList.Entity);

            return View(resultTagList);

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewTag(int id)
        {
            var tagResponse = await _tagService.Get(id);
           
            if (!tagResponse.Success)
            {
                _logger.LogInformation(tagResponse.Message);
                return View("NotFound");
            }

            return View(_mapper.Map<TagViewModel>(tagResponse.Entity));

        }
    }
}
