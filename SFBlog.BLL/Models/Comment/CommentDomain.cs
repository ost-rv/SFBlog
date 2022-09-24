using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{
    public class CommentDomain
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Коментарии к статье обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Коментарии к статье", Prompt = "Введите комментарий")]
        public string Content { get; set; }

        public DateTime DateAdd { get; set; }

        [Display(Name = "Id Статьи")]
        [Range(1, int.MaxValue, ErrorMessage = "Не указан Id статьи")]
        public int PostId { get; set; }

        public int UserId { get; set; }

        public PostDomain? Post { get; set; }
        public UserDomain? User { get; set; }
    }
}


