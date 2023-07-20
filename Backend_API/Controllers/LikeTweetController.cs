using ASP_API.Models;
using Backend_API.Data.Entities;
using Backend_API.Data;
using Microsoft.AspNetCore.Mvc;
using ASP_API.Models.LikeTweet;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ASP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeTweetController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AppEFContext _appEFContext;
        public LikeTweetController(IMapper mapper, AppEFContext appEFContext)
        {
            _appEFContext = appEFContext;
            _mapper = mapper;
        }
        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] LikeTweetViewModel model)
        {
            var result = await _appEFContext.TweetsLikes.AnyAsync(x=> x.UserId == model.UserId && x.TweetId == model.TweetId);
            return Ok(result);
        }

        [HttpGet("list{id}")]
        public async Task<IActionResult> List(int id)
        {
            var result = await _appEFContext.TweetsLikes.Where(x=> x.TweetId == id).ToListAsync();
            return Ok(result);
        }
        [HttpPost("likeTweet")]
        public async Task<IActionResult> Post([FromForm] LikeTweetViewModel model)
        {
            var result = await _appEFContext.TweetsLikes.AnyAsync(x => x.UserId == model.UserId && x.TweetId == model.TweetId);
            if (!result)
            {
                TweetLikeEntity like = _mapper.Map<TweetLikeEntity>(model);
                _appEFContext.AddAsync(like);
                _appEFContext.SaveChangesAsync();

                return Ok(like);
            }
            else
                return BadRequest(406);
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
