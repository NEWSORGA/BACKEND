using Backend_API.Models.Auth;

namespace ASP_API.Models.Comments
{
    public class CommentsCreateViewModel
    {
        public string CommentText { get; set; }
        public int TweetId { get; set; }
        public List<int>? MediaIds { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime? PostTime { get; set; }
        public string TimeZone { get; set; }
    }

    public class CommentsGetViewModel
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public int TweetId { get; set; }
        public List<CommentsViewImageModel>? Images { get; set; }
        public int UserId { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
        public List<CommentsGetViewModel>? CommentsChild { get; set; }

    }
    public class CommentsUploadImageModel
    {
        public IFormFile Media { get; set; }
    }

    public class CommentsViewImageModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string? CommentText { get; set; }
        public List<CommentsViewImageModel>? Medias { get; set; }
        public UserViewModel User { get; set; }
        public int ThoughtId { get; set; }
        public CommentViewModel? Parent { get; set; }
        //public bool Liked { get; set; }
        //public int LikesCount { get; set; }
        //public bool Retweeted { get; set; }
        //public int RetweetedCount { get; set; }
        //public int CommentsCount { get; set; }
        //public int ViewsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
    }

}
