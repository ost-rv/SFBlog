﻿using SFBlog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Models
{
    public class UserDomain
    {
        public UserDomain()
        { }

        public static UserDomain CreateUserDomain(User user)
        {
            return Helper.Mapper.Map<UserDomain>(user);
        }


        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}