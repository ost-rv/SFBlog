using System.ComponentModel.DataAnnotations;

namespace SFBlog.Models
{
    public class CheckTagViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Text)]

        public string Designation { get; set; }
        
        public bool Checked { get; set; }

    }
}
