using Microsoft.AspNetCore.Mvc;
using SFBlog.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class PostEditViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Заголовок обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Заголовок", Prompt = "Введите заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле Текст обязательно для заполнения")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст", Prompt = "Введите текст поста")]
        public string Content { get; set; }

        public List<CheckTagViewModel> CheckTags { get; set; }

    }
}
