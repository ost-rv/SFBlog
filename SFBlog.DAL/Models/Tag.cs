using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.DAL.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Designation { get; set; }

        public ICollection<PostTag> PostTags { get; set; }


    }
}
