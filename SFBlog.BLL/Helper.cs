using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL
{
    public static class Helper
    {

        static Mapper _mapper = new Mapper(new MapperConfiguration( r => r.AddProfile(new MappingProfile())));
        
        public static Mapper Mapper { get { return _mapper; } }
    }
}
