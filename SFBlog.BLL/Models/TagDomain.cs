using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{
    public class TagDomain
    {
        public static TagDomain CreateTagDomain(Tag tag)
        {
            return Helper.Mapper.Map<TagDomain>(tag);
        }

        public int Id { get; set; }
        public string Designation { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
