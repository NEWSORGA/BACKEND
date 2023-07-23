using ASP_API.Models;
using ASP_API.Models.Comments;
using ASP_API.Models.LikeTweet;
using AutoMapper;
using Backend_API.Data.Entities;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;

namespace ASP_API.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<TweetMediaEnitity, TweetViewImageModel>();
            CreateMap<LikeTweetViewModel, TweetLikeEntity>();
            CreateMap<CommentMediaEntity, CommentsViewImageModel>();
            CreateMap<UserEntity, UserViewModel>();
        }
    }
}
