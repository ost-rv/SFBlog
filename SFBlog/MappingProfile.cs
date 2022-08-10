using AutoMapper;
using SFBlog.Models;
using SFBlog.DAL.Models;

namespace SFBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserEditViewModel>();
            CreateMap<User, UserViewModel>();
            
            CreateMap<Post, PostViewModel>();
            CreateMap<Post, PostEditViewModel>();

            CreateMap<Comment, CommentViewModel>();
            CreateMap<Comment, CommentEditViewModel>();

            CreateMap<Tag, TagViewModel>();
            CreateMap<Tag, TagEditViewModel>();




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
