using Backend_API.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Entities
{
    [Table("follows")]
    public class FollowEntity
    {
        public int Id { get; set; }
        [ForeignKey("Follower")]
        public int FollowerId { get; set;}
        public virtual UserEntity Follower { get; set; }
        [ForeignKey("Followed")]
        public int FollowedId { get; set; }
        public virtual UserEntity Followed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
