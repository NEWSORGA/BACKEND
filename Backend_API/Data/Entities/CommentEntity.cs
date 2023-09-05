using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Entities
{
    [Table("comments")]
    public class CommentEntity
    {
        public int Id { get; set; }

        [ForeignKey("Tweet")]
        public int TweetId { get; set; }
        public virtual TweetEntity Tweet { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }

        [ForeignKey("CommentParentId")]
        public int? CommentParentId { get; set; }


        public bool IsComment { get; set; }
        public bool IsReply { get; set; }

        [ForeignKey("ReplyTo")]
        public int? ReplyToId { get; set; }
        public virtual CommentEntity? ReplyTo { get; set; }

        public virtual List<CommentEntity>? CommentsChildren { get; set; }
        public virtual List<CommentMediaEntity>? CommentsMedia { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
