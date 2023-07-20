namespace ASP_API.Models.Comments
{
    public class CommentsCreateViewModel
    {
        public string CommentText { get; set; }
        public int TweetId { get; set; }
        public List<int>? ImagesID { get; set; }
        public int UserId { get; set; }
        public int? CommentParentId { get; set; }


    }
}
