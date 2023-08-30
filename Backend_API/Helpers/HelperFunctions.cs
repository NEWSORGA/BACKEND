using ASP_API.Models;
using Backend_API.Data.Entities;
using Backend_API.Data;
using Backend_API.Models.Auth;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ASP_API.Models.Comments;

namespace ASP_API.Helpers
{
    public class HelperFunctions
    {
        private readonly AppEFContext _appEFContext;
        private readonly IMapper _mapper;
        public HelperFunctions(AppEFContext appEFContext, IMapper mapper)
        {
            _appEFContext = appEFContext;
            _mapper = mapper;
        }
        public List<CommentsGetViewModel> GetChildComments(int parentId)
        {
            var result = _appEFContext.Comments.Where(x => x.CommentParentId == parentId).Select(x => _mapper.Map<CommentsGetViewModel>(x)).ToList();

            return result;
        }

        public static string ConvertDateTimeToStr(DateTime dt)
        {
            if(dt.Minute == 30 && dt.Hour == 14)
            {
                Console.WriteLine("dfas");
            }
            if (dt.Date == DateTime.UtcNow.Date)
            {
                DateTime saveNow = DateTime.Now;

                DateTime saveUtcNow = DateTime.UtcNow;

                Console.WriteLine($"UtcNow: {saveUtcNow.ToString()}");
                Console.WriteLine($"Now: {saveNow.ToString()}");

                if (DateTime.UtcNow.Hour - dt.Hour > 0)
                    return DateTime.UtcNow.Hour - dt.Hour + "h";

                else if (DateTime.UtcNow.Minute - dt.Minute > 0)
                    return DateTime.UtcNow.Minute - dt.Minute + "m";

                else
                    return "now";
            }
            else if (dt.Year == DateTime.UtcNow.Year)
                return $"{char.ToUpper(dt.ToString("MMM")[0]) + dt.ToString("MMM").Substring(1)} {dt.Day.ToString()}";
            else
                return $"{dt.Year.ToString()} {char.ToUpper(dt.ToString("MMM")[0]) + dt.ToString("MMM").Substring(1)} {dt.Day.ToString()}";
        }

        public static TweetViewModel ConvertToModel(TweetEntity tweet, int? UserId, AppEFContext _appEFContext, IMapper _mapper)
        {
        
            TweetViewModel tweetModel = new TweetViewModel
            {
                Id = tweet.Id,
                TweetText = tweet.TweetText,
                Medias = _appEFContext.TweetsMedias.Where(m => m.TweetId == tweet.Id).Select(m => _mapper.Map<TweetViewImageModel>(m)).ToList(),
                User = _mapper.Map<UserViewModel>(tweet.User),
                Reposted = tweet.Reposted == null ? null : ConvertToModel(tweet.Reposted, UserId, _appEFContext, _mapper),
                Liked = UserId == null ? false : _appEFContext.TweetsLikes.SingleOrDefault(l => l.UserId == UserId && l.TweetId == tweet.Id) == null ? false : true,
                LikesCount = _appEFContext.TweetsLikes.Where(l => l.TweetId == tweet.Id).ToList().Count,
                Retweeted = UserId == null ? false : _appEFContext.Tweets.SingleOrDefault(t => t.UserId == UserId && t.RepostedId == tweet.Id) == null ? false : true,
                RetweetedCount = _appEFContext.Tweets.Where(t => t.RepostedId == tweet.Id).ToList().Count,
                CommentsCount = _appEFContext.Comments.Where(c => c.TweetId == tweet.Id).ToList().Count,
                ViewsCount = tweet.Views,
                CreatedAt = tweet.CreatedAt,
                CreatedAtStr = ConvertDateTimeToStr(tweet.PostTime)
            };

            return tweetModel;
        }

        public static List<CommentViewModel> GetCommentsWithChildren(int? parentCommentId, int? thoughtId, AppEFContext _appEFContext, IMapper _mapper)
        {
            var comments = new List<CommentEntity>();
            if (thoughtId != null)
            {
                comments = _appEFContext.Comments
                    .Include(c => c.User)
                    .Where(c => c.TweetId == thoughtId && c.CommentParentId == null)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }
            else
            {
                comments = _appEFContext.Comments
                    .Include(c => c.User)
                    .Where(c => c.CommentParentId == parentCommentId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }

            var commentsWithChildren = new List<CommentViewModel>();

            foreach (var comment in comments)
            {
                var commentModel = new CommentViewModel
                {
                    // Заповнюємо дані з коментаря, які не змінюються

                    Id = comment.Id,
                    CommentText = comment.CommentText,
                    Medias = _appEFContext.CommentsMedias.Where(m => m.CommentId == comment.Id).Select(m => _mapper.Map<CommentsViewImageModel>(m)).ToList(),
                    User = _mapper.Map<UserViewModel>(comment.User),
                    ThoughtId = comment.TweetId,
                    CreatedAt = comment.CreatedAt,
                    CreatedAtStr = HelperFunctions.ConvertDateTimeToStr(comment.CreatedAt),
                    Children = GetCommentsWithChildren(comment.Id, null, _appEFContext, _mapper)
                };

                commentsWithChildren.Add(commentModel);
            }

            return commentsWithChildren;
        }

        public static async Task<TweetViewModel> ConvertToModel2(TweetEntity tweet, int? UserId, AppEFContext _appEFContext, IMapper _mapper)
        {
            var likesCount = await _appEFContext.TweetsLikes.Where(l => l.TweetId == tweet.Id).ToListAsync();
            var retweetedCount = await _appEFContext.Tweets.Where(t => t.RepostedId == tweet.Id).ToListAsync();
            var commentsCount = await _appEFContext.Comments.Where(c => c.TweetId == tweet.Id).ToListAsync();
            TweetViewModel tweetModel = new TweetViewModel
            {
                Id = tweet.Id,
                TweetText = tweet.TweetText,
                Medias = await _appEFContext.TweetsMedias.Where(m => m.TweetId == tweet.Id).Select(m => _mapper.Map<TweetViewImageModel>(m)).ToListAsync(),
                User = _mapper.Map<UserViewModel>(tweet.User),
                Reposted = ConvertToModel2(tweet.Reposted, UserId, _appEFContext, _mapper).Result,
                Liked = UserId == null ? false : _appEFContext.TweetsLikes.SingleOrDefault(l => l.UserId == UserId && l.TweetId == tweet.Id) == null ? false : true,
                LikesCount = likesCount.Count,
                Retweeted = UserId == null ? false : _appEFContext.Tweets.SingleOrDefault(t => t.UserId == UserId && t.RepostedId == tweet.Id) == null ? false : true,
                RetweetedCount = retweetedCount.Count,
                CommentsCount = commentsCount.Count,
                ViewsCount = tweet.Views,
                CreatedAt = tweet.CreatedAt,
                CreatedAtStr = ConvertDateTimeToStr(tweet.PostTime)
            };

            return tweetModel;
        }

        public static void DeleteMedia(string path, IConfiguration _configuration)
        {      
                var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                string[] sizes = ((string)_configuration.GetValue<string>("ImageSizes")).Split(" ");
                foreach (var s in sizes)
                {
                    int size = Convert.ToInt32(s);
                    System.IO.File.Delete(Path.Combine(dirSave, s + "_" + path));
                }

        }

       
    }
}
