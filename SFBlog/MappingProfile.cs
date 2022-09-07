using AutoMapper;
using SFBlog.Models;
using SFBlog.DAL.Models;
using SFBlog.BLL.Models;
using System.Linq;

namespace SFBlog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserEditViewModel>();
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.Roles, src => src.MapFrom(c => c.UserRoles));
            
            CreateMap<Post, PostViewModel>()
                .ForMember(dst => dst.Tags, src => src.MapFrom(c => c.PostTags))
                .ForMember(dst => dst.Author, src => src.MapFrom(c => c.User.Email));
            CreateMap<Post, PostEditViewModel>();
            CreateMap<Post, PostLightViewModel>();
            CreateMap<PostEditViewModel, Post>()
                .ForMember(dst => dst.PostTags, 
                           src => src.MapFrom(c => c.CheckTags
                                                .Where(t => t.Checked)
                                                .Select(t => new PostTag { TagId = t.Id})));

            CreateMap<PostTag, TagViewModel>()
                .ForMember(dst => dst.Id, src => src.MapFrom(c => c.TagId))
                .ForMember(dst => dst.Designation, src => src.MapFrom(c => c.Tag.Designation));
            CreateMap<PostTag, TagLightViewModel>()
                .ForMember(dst => dst.Id, src => src.MapFrom(c => c.TagId))
                .ForMember(dst => dst.Designation, src => src.MapFrom(c => c.Tag.Designation));

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dst => dst.Author, src => src.MapFrom(c => c.User.Email));
            CreateMap<Comment, CommentEditViewModel>();

            CreateMap<Tag, TagViewModel>()
                .ForMember(dst => dst.Posts, srs => srs.MapFrom(c => c.PostTags));
            CreateMap<Tag, TagEditViewModel>();

            CreateMap<TagDomain, TagViewModel>();

            CreateMap<Role, RoleViewModel>();
            CreateMap<Role, RoleEditViewModel>();
            
            CreateMap<UserRole, RoleViewModel>()
                .ForMember(dst => dst.Id, src => src.MapFrom(c => c.RoleId))
                .ForMember(dst => dst.Name, src => src.MapFrom(c => c.Role.Name));

        }
    }
}
