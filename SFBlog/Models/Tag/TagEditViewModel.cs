using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class TagEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название")]
        public string Designation { get; set; }

    }
}
