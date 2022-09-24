using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{
    public class TagDomain
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название")]
        public string Designation { get; set; }

        public ICollection<PostDomain> Posts { get; set; }
    }
}
