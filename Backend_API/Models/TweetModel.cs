using Backend_API.Data.Entities;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;

namespace ASP_API.Models
{
    public class TweetCreateModel
    {
        public string TweetText { get; set; }
        public int UserId { get; set; }
        public int? RepostedId { get; set; }
        public int[] MediaIds { get; set; }
    }
    public class TweetViewModel
    {
        public int Id { get; set; }
        public string TweetText { get; set; }
        public List<TweetViewImageModel> ? Medias { get; set; }
        public UserViewModel User { get; set; }
        public TweetViewModel ? Reposted { get; set; }
        public bool Liked { get; set; }
        public int LikesCount { get; set; }
        public bool Retweeted { get; set; }
        public int RetweetedCount { get; set; }
        public int CommentsCount { get; set; }
        public int ViewsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
    }

    public class TweetUploadImageModel
    {
        public IFormFile Media { get; set; }
    }

    public class TweetViewImageModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
}
