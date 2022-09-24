using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{
    public class PostDomain
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Заголовок обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Заголовок", Prompt = "Введите заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле Текст обязательно для заполнения")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст", Prompt = "Введите текст поста")]
        public string Content { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateAdd { get; set; }
        public int UserId { get; set; }

        public UserDomain? User { get; set; }

        public ICollection<TagDomain> Tags { get; set; }
        public ICollection<CommentDomain> Comments { get; set; }

    }
}

