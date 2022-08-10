using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class CommentEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Комментарий обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Комментарий", Prompt = "Введите комментарий")]
        public string Content { get; set; }

    }
}
