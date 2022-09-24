using AutoMapper;
using SFBlog.DAL.Models;
using SFBlog.BLL.Models;


namespace SFBlog.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            CreateMap<User, UserDomain>()
                .ForMember(u => u.Roles, x => x.MapFrom(m => m.UserRoles.Select(ur => ur.Role)));
            CreateMap<UserDomain, User>()
                .ForMember(u => u.UserRoles, x => x.MapFrom(m => m.Roles.Select(r => new UserRole { UserId = m.Id, User = null, RoleId = r.Id, Role = null })));


            //Role
            CreateMap<Role, RoleDomain>()
                .ForMember(r => r.Users, x => x.MapFrom(m => m.UserRoles.Select(ur => ur.User)));
            CreateMap<RoleDomain, Role>()
                .ForMember(u => u.UserRoles, x => x.MapFrom(m => m.Users.Select(r => new UserRole { UserId = r.Id, User = null, RoleId = r.Id, Role = null })));


            //Tag
            CreateMap<Tag, TagDomain>()
                .ForMember(dst => dst.Posts, x => x.MapFrom(m => m.PostTags.Select(ur => ur.Post)));
            CreateMap<TagDomain, Tag>()
                .ForMember(dst => dst.PostTags, x => x.MapFrom(m => m.Posts.Select(src => new PostTag { PostId = src.Id, TagId = m.Id })));

            //Comment
            CreateMap<Comment, CommentDomain>();
            CreateMap<CommentDomain, Comment>()
                .ForMember(dst => dst.Post, src => src.MapFrom(m => m.Post))
                .ForMember(dst => dst.User, src => src.MapFrom(m => m.User));

            //Post
            CreateMap<Post, PostDomain>()
                .ForMember(dst => dst.Tags, src => src.MapFrom(m => m.PostTags.Select(pt => pt.Tag)));
            CreateMap<PostDomain, Post>()
                .ForMember(dst => dst.PostTags, src => src.MapFrom(m => m.Tags.Select(t => new PostTag { PostId = m.Id, TagId = t.Id  } )));
        }
    }
}
