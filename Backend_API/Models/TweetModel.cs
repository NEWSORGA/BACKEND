namespace ASP_API.Models
{
    public class TweetCreateModel
    {
        public string TweetText { get; set; }
        public int UserId { get; set; }
        public int? RepostedId { get; set; }
        public int[] MediaIds { get; set; }
    }

    public class TweetUploadImageModel
    {
        public IFormFile Media { get; set; }
    }

    public class TweetViewImageModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
}
