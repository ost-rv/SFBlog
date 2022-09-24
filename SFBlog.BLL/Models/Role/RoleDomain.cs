using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{ 
    public class RoleDomain
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Обозначение обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }

        public ICollection<UserDomain> Users { get; set; }

    }
}

