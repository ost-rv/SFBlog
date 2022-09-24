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
            //User
            CreateMap<UserDomain, UserRegisterViewModel>();
            CreateMap<UserRegisterViewModel, UserDomain>();

            CreateMap<UserDomain, UserEditViewModel>();
            CreateMap<UserEditViewModel, UserDomain>()
                .ForMember(dst => dst.Roles,
                           src => src.MapFrom(c => c.CheckRoles
                                    .Where(r => r.Checked)
                                    .Select(r => new RoleDomain { Id = r.Id, Name = r.Name})));

            CreateMap<UserDomain, UserViewModel>()
                .ForMember(dst => dst.Roles, src => src.MapFrom(c => c.Roles));


            //Post
            CreateMap<PostDomain, PostViewModel>()
                .ForMember(dst => dst.Tags, src => src.MapFrom(c => c.Tags))
                .ForMember(dst => dst.Author, src => src.MapFrom(c => c.User.Email));
            
            CreateMap<PostDomain, PostEditViewModel>();
            CreateMap<PostDomain, PostLightViewModel>();
            CreateMap<PostEditViewModel, PostDomain>()
                .ForMember(dst => dst.Tags, 
                           src => src.MapFrom(c => c.CheckTags
                                                .Where(t => t.Checked)
                                                .Select(t => new TagDomain { Id = t.Id})));

            //Tag
            CreateMap<TagDomain, TagViewModel>();
            CreateMap<TagDomain, TagLightViewModel>();
            CreateMap<TagDomain, TagEditViewModel>();
            CreateMap<TagEditViewModel, TagDomain>();

            //Comment
            CreateMap<CommentDomain, CommentViewModel>()
                .ForMember(dst => dst.Author, src => src.MapFrom(c => c.User.Email));
            CreateMap<CommentDomain, CommentEditViewModel>();
            CreateMap<CommentEditViewModel, CommentDomain> ();


            //Role
            CreateMap<RoleDomain, RoleViewModel>();
            CreateMap<RoleDomain, RoleEditViewModel>();
            CreateMap<RoleEditViewModel, RoleDomain>();
        }
    }
}
