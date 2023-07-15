using ASP_API.Models;
using AutoMapper;
using Backend_API.Data.Entities;

namespace ASP_API.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<TweetMediaEnitity, TweetViewImageModel>();
               
        }
    }
}
