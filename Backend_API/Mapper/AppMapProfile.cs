using ASP_API.Helpers;
using ASP_API.Models;
using ASP_API.Models.Comments;
using ASP_API.Models.LikeTweet;
using AutoMapper;
using Backend_API.Data;
using Backend_API.Data.Entities;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace ASP_API.Mapper
{
    public class AppMapProfile : Profile
    {
        

        public AppMapProfile()
        {
            CreateMap<TweetMediaEnitity, TweetViewImageModel>();
            CreateMap<CommentMediaEntity, CommentsViewImageModel>();
            CreateMap<LikeTweetViewModel, TweetLikeEntity>();
            CreateMap<CommentMediaEntity, CommentsViewImageModel>();
            CreateMap<UserEntity, UserViewModel>().ForMember(s => s.IsFollowed, opt => opt.MapFrom(s => false));
            CreateMap<CommentEntity, CommentsGetViewModel>().ForMember(s => s.CreatedAtStr, opt => opt.MapFrom(s => HelperFunctions.ConvertDateTimeToStr(s.CreatedAt)))
                .ForMember(dest => dest.CommentsChild, opt => opt.MapFrom(src => src.CommentsChildren));
            CreateMap<CommentMediaEntity, CommentsViewImageModel>();
        }

        
    }
}
