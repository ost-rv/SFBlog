using AutoMapper;
using SFBlog.DAL.Models;
using SFBlog.BLL.Models;

namespace SFBlog.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDomain>()
                .ForMember(u => u.Roles, x => x.MapFrom(m => m.UserRoles.Select(ur => ur.Role)));

            CreateMap<Tag, TagDomain>()
                .ForMember(u => u.Posts, x => x.MapFrom(m => m.PostTags.Select(ur => ur.Post)));



            //CreateMap<RegisterViewModel, User>()
            //    .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Day)))
            //    .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
            //    .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login));
            //CreateMap<LoginViewModel, User>();

            //CreateMap<UserEditViewModel, User>();
            //CreateMap<User, UserEditViewModel>().ForMember(x => x.UserId, opt => opt.MapFrom(c => c.Id));

            //CreateMap<UserWithFriendExt, User>();
            //CreateMap<User, UserWithFriendExt>();
        }
    }
}
