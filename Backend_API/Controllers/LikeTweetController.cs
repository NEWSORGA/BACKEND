using ASP_API.Models;
using Backend_API.Data.Entities;
using Backend_API.Data;
using Microsoft.AspNetCore.Mvc;
using ASP_API.Models.LikeTweet;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Backend_API.Data.Entities.Identity;

namespace ASP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeTweetController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AppEFContext _appEFContext;
        private readonly UserManager<UserEntity> _userManager;
        public LikeTweetController(IMapper mapper, AppEFContext appEFContext, UserManager<UserEntity> userManager)
        {
            _appEFContext = appEFContext;
            _mapper = mapper;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] LikeTweetViewModel model)
        {
            var user = User;
            
            var result = await _appEFContext.TweetsLikes.AnyAsync(x=> x.UserId == model.UserId && x.TweetId == model.TweetId);
            return Ok(result);
        }

        [HttpGet("list{id}")]
        public async Task<IActionResult> List(int id)
        {
            var result = await _appEFContext.TweetsLikes.Where(x=> x.TweetId == id).ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

            var result = await _appEFContext.TweetsLikes.SingleOrDefaultAsync(x => x.UserId == user.Id && x.TweetId == id);
            if (result == null)
            {
                TweetLikeEntity like = new TweetLikeEntity
                {
                    TweetId = id,
                    UserId = user.Id
                };
                _appEFContext.AddAsync(like);
                _appEFContext.SaveChangesAsync();


                return Ok("Liked");
            }
            else
            {
                _appEFContext.Remove(result);
                _appEFContext.SaveChangesAsync();
                return Ok("unLiked");
            }
            return BadRequest();

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            
            var like = await _appEFContext.TweetsLikes.SingleOrDefaultAsync(x => x.Id == id);
            _appEFContext.TweetsLikes.Remove(like);
            _appEFContext.SaveChangesAsync();
            return Ok();
        }
    }
}
