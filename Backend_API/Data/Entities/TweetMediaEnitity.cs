using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Backend_API.Data.Entities
{
    [Table("tweets_media")]
    public class TweetMediaEnitity
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Tweet")]
        public int ? TweetId { get; set; }
        public virtual TweetEntity Tweet { get; set; }
    }
}
