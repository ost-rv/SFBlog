using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;

namespace SFBlog.Models
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string Designation { get; set; }

        public ICollection<PostLightViewModel> Posts { get; set; }

        public int CountPost => Posts?.Count ?? 0;
    }
}
