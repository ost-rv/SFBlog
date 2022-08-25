using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.DAL.Models
{
    public class Post
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int ViewCount { get; set; }

        public DateTime DateAdd { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ObservableCollection<PostTag> PostTags { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }


        public Post()
        { 
            PostTags = new ObservableCollection<PostTag>();
        }

    }
}
