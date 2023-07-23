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
    public class CommentsUploadImageModel
    {
        public IFormFile Media { get; set; }
    }

    public class CommentsViewImageModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
}
