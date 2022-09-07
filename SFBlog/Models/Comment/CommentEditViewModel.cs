using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class CommentEditViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Комментарий обязательно для заполнения")]
        
        [Display(Name = "Id Статьи")]
        [HiddenInput]
        [Range(1, int.MaxValue, ErrorMessage = "Не указан Id статьи")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Поле Коментарии к статье обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Коментарии к статье", Prompt = "Введите комментарий")]
        public string Content { get; set; }

    }
}
