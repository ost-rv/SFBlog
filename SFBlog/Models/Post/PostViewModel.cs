using System;
using System.Collections.Generic;

namespace SFBlog.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string Author { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateAdd { get; set; }

        public List<TagLightViewModel> Tags { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}
