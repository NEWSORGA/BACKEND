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
        public TweetsController(AppEFContext appEFContext, IConfiguration configuration, IMapper mapper)
        {
            _appEFContext = appEFContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        // GET: api/<TweetsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TweetsController>/5
        [HttpGet("GetUserTweets")]
        public async Task<IActionResult> GetUserTweets(int? UserId, int UserPageId) //перше користувач, який авторизований, другий параметр - користувач, чиї твіти отримати
        {
            var models = await _appEFContext.Tweets
                .Where(s => s.UserId == UserPageId)
                .Include(x => x.User)
                .Include(x => x.Reposted)
                .ToListAsync();

            List<TweetViewModel> tweets = new List<TweetViewModel>();

            foreach (TweetEntity item in models)
            {
                tweets.Add(HelperFunctions.ConvertToModel(item, UserId, _appEFContext, _mapper));
            }

            return Ok(tweets);   
        }

        
        // POST api/<TweetsController>
   
        [HttpPost("CreateTweet")]
        public async Task<IActionResult> Post([FromForm] TweetCreateModel model)
        {
          
                var tweet = new TweetEntity()
                {
                    TweetText = model.TweetText,
                    UserId = model.UserId,
                    RepostedId = model.RepostedId,
                    Views = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _appEFContext.AddAsync(tweet);
                await _appEFContext.SaveChangesAsync();


                if (model.MediaIds.Length > 0)
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
                
            _appEFContext.TweetsMedias.Remove(item);
            _appEFContext.SaveChanges();

            return Ok("Deleted");

        }

        // PUT api/<TweetsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE api/<TweetsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
           
        }
    }
}
