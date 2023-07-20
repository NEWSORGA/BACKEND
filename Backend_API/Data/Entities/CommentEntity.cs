using Backend_API.Data.Entities.Identity;
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

        [ForeignKey("CommentParent")]
        public int? CommentParentId { get; set; }
        public virtual CommentEntity? CommentParent { get; set; }

        public virtual List<CommentEntity>? CommentsChildren { get; set; }
        public virtual List<CommentMediaEntity>? CommentsMedia { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
