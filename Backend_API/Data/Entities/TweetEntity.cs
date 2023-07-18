using Backend_API.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Entities
{
    [Table("tweets")]
    public class TweetEntity
    {
        public int Id { get; set; }
        public string TweetText { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }
        [ForeignKey("Reposted")]
        public int? RepostedId { get; set; }
        public virtual TweetEntity Reposted { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
