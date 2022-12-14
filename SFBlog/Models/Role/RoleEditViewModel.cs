using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class RoleEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Обозначение обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }

    }
}
