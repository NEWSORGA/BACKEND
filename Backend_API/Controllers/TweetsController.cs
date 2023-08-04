using ASP_API.Helpers;
using ASP_API.Models;
using AutoMapper;
using Backend_API.Asbtract;
using Backend_API.Constans;
using Backend_API.Data;
using Backend_API.Data.Entities;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly AppEFContext _appEFContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        public TweetsController(AppEFContext appEFContext, IConfiguration configuration, IMapper mapper, UserManager<UserEntity> userManager)
        {
            _appEFContext = appEFContext;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/<TweetsController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, int? UserId)
        {
            var item = await _appEFContext.Tweets
                .Include(x => x.User)
                .Include(x => x.Reposted)
                .SingleOrDefaultAsync();

              var result = HelperFunctions.ConvertToModel(item, UserId, _appEFContext, _mapper);

            return Ok(result);
        }

        // GET api/<TweetsController>/5
        [HttpGet("GetUserTweets")]
        public async Task<IActionResult> GetUserTweets(int? UserId, int UserPageId) //перше користувач, який авторизований, другий параметр - користувач, чиї твіти отримати
        {

            var models = await _appEFContext.Tweets
                .Where(s => s.UserId == UserPageId)
                .Include(x => x.User)
                .Include(x => x.Reposted)
                .Where(x => DateTime.Compare(x.PostTime, DateTime.UtcNow) < 0)
                .OrderByDescending(x => x.PostTime)
                .ToListAsync();

            //models.Sort((x, y) => DateTime.Compare(x.PostTime, y.PostTime));
            List<TweetViewModel> tweets = new List<TweetViewModel>();

            foreach (TweetEntity item in models)
            {
                tweets.Add(HelperFunctions.ConvertToModel(item, UserId, _appEFContext, _mapper));
            }

            return Ok(tweets);   
        }


        // POST api/<TweetsController>
        [Authorize]
        [HttpPost("CreateTweet")]
        public async Task<IActionResult> Post([FromForm] TweetCreateModel model)
        {
            if ((model.TweetText == null || model.TweetText == "") && model.MediaIds == null && model.RepostedId == null)
                return BadRequest("All fields are empty!");
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

            DateTime postTime = model.PostTime == null ? DateTime.UtcNow.AddSeconds(-10) : model.PostTime.Value;
     
            var tweet = new TweetEntity()
                {
                    TweetText = model.TweetText,
                    UserId = user.Id,
                    RepostedId = model.RepostedId,
                    Views = 0,
                    PostTime = postTime,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _appEFContext.AddAsync(tweet);
                await _appEFContext.SaveChangesAsync();


                if (model.MediaIds != null)
                {
                    foreach (int mediaId in model.MediaIds)
                    {
                        var image = await _appEFContext.TweetsMedias.SingleAsync(s => s.Id == mediaId);
                        image.TweetId = tweet.Id;
                       
                    }
                    await _appEFContext.SaveChangesAsync();
                }

                return Ok("Done");         
        }
        
        [HttpPost("uploadMedia")]
        public async Task<IActionResult> UploadMedia([FromForm] TweetUploadImageModel model)
        {
            //fdasfsdfsafssssssss
            string imageName = string.Empty;
            if (model.Media != null)
            {
                var fileExp = Path.GetExtension(model.Media.FileName);
                var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                imageName = Path.GetRandomFileName() + fileExp;
                using (var ms = new MemoryStream())
                {
                    await model.Media.CopyToAsync(ms);
                    var bmp = new Bitmap(System.Drawing.Image.FromStream(ms));
                    string[] sizes = ((string)_configuration.GetValue<string>("ImageSizes")).Split(" ");
                    foreach (var s in sizes)
                    {
                        int size = Convert.ToInt32(s);
                        var saveImage = ImageWorker.CompressImage(bmp, size, size, false);
                        saveImage.Save(Path.Combine(dirSave, s + "_" + imageName));
                    }
                }
                var entity = new TweetMediaEnitity();
                entity.Path = imageName;
                entity.CreatedAt = DateTime.UtcNow;

                _appEFContext.TweetsMedias.Add(entity);
                _appEFContext.SaveChanges();
                return Ok(_mapper.Map<TweetViewImageModel>(entity));

            }
            return BadRequest();
        }

        [HttpDelete("deleteMedia")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var item = await _appEFContext.TweetsMedias.SingleOrDefaultAsync(s => s.Id == id);
            HelperFunctions.DeleteMedia(item.Path, _configuration);
            _appEFContext.TweetsMedias.Remove(item);
            _appEFContext.SaveChanges();

            return Ok("Deleted");

        }

        // PUT api/<TweetsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] TweetEditModel model)
        {
            var tweet = await _appEFContext.Tweets.SingleOrDefaultAsync(t => t.Id == id);
            tweet.TweetText = model.TweetText;
            tweet.RepostedId = model.RepostedId;
            tweet.UpdatedAt = DateTime.UtcNow;
       


            if (model.MediaDeletedIds.Length > 0)
            {
                foreach (int mediaId in model.MediaDeletedIds)
                {
                    var image = await _appEFContext.TweetsMedias.SingleAsync(s => s.Id == mediaId);
                    HelperFunctions.DeleteMedia(image.Path, _configuration);
                    _appEFContext.Remove(image);
                }
            }

            if (model.AddedMediaIds.Length > 0)
            {
                foreach (int mediaId in model.AddedMediaIds)
                {
                    var image = await _appEFContext.TweetsMedias.SingleAsync(s => s.Id == mediaId);
                    image.TweetId = tweet.Id;

                }
            }

            _appEFContext.Update(tweet);
            await _appEFContext.SaveChangesAsync();


            return Ok(tweet);
        }

        // DELETE api/<TweetsController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

            var tweet = await _appEFContext.Tweets.SingleOrDefaultAsync(s => s.Id == id);
            var medias = await _appEFContext.TweetsMedias.Where(s => s.TweetId == id).ToListAsync();
            var comments = await _appEFContext.Comments.Where(s => s.TweetId == id).ToListAsync();
            var likes = await _appEFContext.TweetsLikes.Where(s => s.TweetId == id).ToListAsync();

            if (user.Id != tweet.UserId)
                return BadRequest("Its not your post!");

            foreach (var item in comments)
            {
                var mediaComs = await _appEFContext.CommentsMedias.Where(s => s.CommentId == item.Id).ToListAsync();
                foreach (var media in medias)
                {
                    HelperFunctions.DeleteMedia(media.Path, _configuration);
                    _appEFContext.Remove(media);
                }
            }
            foreach (var media in medias)
            {
                HelperFunctions.DeleteMedia(media.Path, _configuration);
                _appEFContext.Remove(media);
            }
            
            _appEFContext.Remove(tweet);

            await _appEFContext.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
