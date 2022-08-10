using System;

namespace SFBlog.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateAdd { get; set; }
    }
}
