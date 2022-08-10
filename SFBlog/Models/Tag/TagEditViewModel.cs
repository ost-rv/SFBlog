using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class TagEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Обозначение обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Обозначение", Prompt = "Введите обозначение")]
        public string Designation { get; set; }

    }
}
