using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class RoleAddViewModel
    {
        [Required(ErrorMessage = "Поле Наименование обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        public string Name { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }

    }
}
