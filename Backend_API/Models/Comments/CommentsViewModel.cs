using Backend_API.Data.Entities;
using Backend_API.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_API.Models.Comments
{
    public class CommentsCreateViewModel
    {
        public string? CommentText { get; set; }
        public int TweetId { get; set; }
        public List<int>? MediaIds { get; set; }
    }
    public class CommentReplyCreateViewModel
    {
        public string? CommentText { get; set; }
        public int TweetId { get; set; }
        public int? ReplyToId { get; set; }
        public int? CommentParentId { get; set; }
        public List<int>? MediaIds { get; set; }
        public bool ReplyToChild { get; set; }
    }

    public class CommentsGetViewModel
    {
        public int Id { get; set; }
        public string? CommentText { get; set; }
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
        public int? CommentParentId { get; set; }
        public bool IsComment { get; set; }
        public bool IsReply { get; set; }
        [ForeignKey("ReplyTo")]
        public int? ReplyToId { get; set; }
        public CommentViewModel? ReplyTo { get; set; }
        public List<CommentViewModel>? Children { get; set; }
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
