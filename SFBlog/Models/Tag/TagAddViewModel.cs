using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class TagAddViewModel
    {
        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название")]
        public string Designation { get; set; }

    }
}
