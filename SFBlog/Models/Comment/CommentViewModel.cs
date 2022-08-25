using System;

namespace SFBlog.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime DateAdd { get; set; }

        public PostLightViewModel Post { get; set; }
    }
}
