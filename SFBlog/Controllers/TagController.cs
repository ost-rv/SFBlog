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
    public class TagController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _UoW;
        private Repository<Tag> _tagRepository;

        public TagController(IMapper mapper, IUnitOfWork UoW)
        {
            _mapper = mapper;
            _UoW = UoW;
            _tagRepository = (Repository<Tag>)_UoW.GetRepository<Tag>();
        }

        [Route("AddTag")]
        [HttpPost]
        public async Task<string> Add(TagEditViewModel newTag, int userId)
        {
            if (ModelState.IsValid)
            {
                var tag = _mapper.Map<Tag>(newTag);


                await _tagRepository.Create(tag);
                return "Успех!";
            }
            return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
        }


        [Authorize]
        [Route("EditTag")]
        [HttpPut]
        public async Task<string> Update(TagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = await _tagRepository.Get(model.Id);

                await _tagRepository.Update(tag);

                return "Успех!";
            }
            else
            {
                return string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors));
            }
        }


        [Authorize]
        [Route("DeleteTag")]
        [HttpDelete]
        public async Task<string> Delete(int tagId)
        {
            Tag tag = await _tagRepository.Get(tagId);
            if (tag is null)
            {
                return $"Тэг с Id = {tagId} не найден";
            }

            await _tagRepository.Delete(tag);
            return "Тэг удален.";
        }

        [Authorize]
        [HttpGet]
        [Route("TagList")]
        public List<TagViewModel> GetTagList()
        {
            List<TagViewModel> resultTagList = new List<TagViewModel>();

            var tagList = _tagRepository.GetAll();

            foreach (Tag tag in tagList)
            {
                resultTagList.Add(_mapper.Map<TagViewModel>(tag));
            }

            return resultTagList;
        }

        [Authorize]
        [HttpGet]
        [Route("Tag")]
        public async Task<TagViewModel> GetTag(int tagId)
        {
            TagViewModel resultTag = new TagViewModel();

            Tag tag = await _tagRepository.Get(tagId);

            return _mapper.Map<TagViewModel>(tag);
        }
    }
}
