using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Entities
{
    [Table("comments_media")]
    public class CommentMediaEntity
    {
        public int Id { get; set; }
        [ForeignKey("Comment")]
        public int CommentId { get; set; }
        public virtual CommentEntity Comment { get; set; }
        public string Path { get; set; }
    }
}
