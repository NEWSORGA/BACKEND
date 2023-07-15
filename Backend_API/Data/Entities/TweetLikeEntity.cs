using Backend_API.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Entities
{
    [Table("tweets_likes")]
    public class TweetLikeEntity
    {
        public int Id { get; set; }
        [ForeignKey("Tweet")]
        public int TweetId { get; set; }
        public virtual TweetEntity Tweet { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
