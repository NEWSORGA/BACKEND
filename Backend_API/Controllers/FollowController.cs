using ASP_API.Models.LikeTweet;
using AutoMapper;
using Backend_API.Data.Entities.Identity;
using Backend_API.Data.Entities;
using Backend_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AppEFContext _appEFContext;
        private readonly UserManager<UserEntity> _userManager;
        public FollowController(IMapper mapper, AppEFContext appEFContext, UserManager<UserEntity> userManager)
        {
            _appEFContext = appEFContext;
            _mapper = mapper;
            _userManager = userManager;
        }
        //[Authorize]
        //[HttpPost("check")]
        //public async Task<IActionResult> Check([FromBody] LikeTweetViewModel model)
        //{
        //    var user = User;

        //    var result = await _appEFContext.TweetsLikes.AnyAsync(x => x.UserId == model.UserId && x.TweetId == model.TweetId);
        //    return Ok(result);
        //}

        //[HttpGet("list{id}")]
        //public async Task<IActionResult> List(int id)
        //{
        //    var result = await _appEFContext.TweetsLikes.Where(x => x.TweetId == id).ToListAsync();
        //    return Ok(result);
        //}

        [Authorize]
        [HttpPost("{UserId}")]
        public async Task<IActionResult> Post(int UserId)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var Follower = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == UserId);

            var result = await _appEFContext.Follows.SingleOrDefaultAsync(x => x.FollowerId == Follower.Id && x.UserId == UserId);
            if (result == null)
            {
                FollowEntity follow = new FollowEntity
                {
                    FollowerId = Follower.Id,
                    UserId = UserId
                };
                user.Followers++;
                Follower.Following++;
                await _appEFContext.AddAsync(follow);
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateAsync(Follower);
                await _appEFContext.SaveChangesAsync();


                return Ok("Followed");
            }
            else
            {
                user.Followers--;
                Follower.Following--;
                _appEFContext.Remove(result);
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateAsync(Follower);
                await _appEFContext.SaveChangesAsync();
                return Ok("unFollowed");
            }
            return BadRequest();

        }

        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{

        //    var like = await _appEFContext.TweetsLikes.SingleOrDefaultAsync(x => x.Id == id);
        //    _appEFContext.TweetsLikes.Remove(like);
        //    _appEFContext.SaveChangesAsync();
        //    return Ok();
        //}
    }
}
