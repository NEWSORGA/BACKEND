using AutoMapper;
using Backend_API.Asbtract;
using Backend_API.Constans;
using Backend_API.Data;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using Backend_API.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        private readonly AppEFContext _appEFContext;
        public AuthController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService, IMapper mapper, AppEFContext appEFContext)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _appEFContext = appEFContext;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, int? ForUser)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == id);

            if(user != null)
            {
                var res = _mapper.Map<UserViewModel>(user);
                res.Followers = _appEFContext.Follows.Where(f => f.UserId == id).ToList().Count;
                res.Following = _appEFContext.Follows.Where(f => f.FollowerId == id).ToList().Count;
                res.Likes = _appEFContext.TweetsLikes.Include(f => f.Tweet).Where(f => f.Tweet.UserId == id).ToList().Count;
                if (ForUser != null)
                {
                    var followed = await _appEFContext.Follows.SingleOrDefaultAsync(f => f.FollowerId == ForUser && f.UserId == id);
                    if (followed != null)
                        res.IsFollowed = true;
                }
                
                return Ok(res);
            }
                
            return BadRequest("User not found!");
        }

        [HttpGet("searchUser")]
        public async Task<IActionResult> SearchUser([FromQuery] string filterText)
        {
            var user = await _appEFContext.Users.Where(u => u.Name.Contains(filterText)).ToListAsync();

            return Ok(user);
        }
        // POST api/<AuthController>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateUserViewModel model)
        {
            System.String imageName = string.Empty;
            if (model.Image != null)
            {
                var fileExp = Path.GetExtension(model.Image.FileName);
                var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                imageName = Path.GetRandomFileName() + fileExp;
                using (var steam = System.IO.File.Create(Path.Combine(dirSave, imageName)))
                {
                    await model.Image.CopyToAsync(steam);
                }
            }

            System.String bgImageName = string.Empty;
            if (model.BackgroundImage != null)
            {
                var fileExp = Path.GetExtension(model.BackgroundImage.FileName);
                var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                bgImageName = Path.GetRandomFileName() + fileExp;
                using (var steam = System.IO.File.Create(Path.Combine(dirSave, bgImageName)))
                {
                    await model.Image.CopyToAsync(steam);
                }
            }

            var user = new UserEntity()
            {
                Name = model.Name,
                Email = model.Email,
                Image = imageName,
                UserName = "user" + _userManager.Users.Count()+1.ToString(),
                Description = "",
                Verified = false,
                Country = model.Country,
                CountryCode = model.CountryCode,
                BackgroundImage = bgImageName
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                var id = await _userManager.GetUserIdAsync(user);
                user.UserName = "user" + id.ToString();
                await _userManager.UpdateAsync(user);
                result = await _userManager.AddToRoleAsync(user, Roles.User);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("createGoogle")]
        public async Task<IActionResult> CreateGoogle([FromForm] CreateGoogleUserViewModel model)
        {
            var users = _userManager.Users.Where(u => u.Email == model.Email).ToList();
            if(users.Count == 0)
            {
                var client = new HttpClient();
                string fileName = null;
                string extension = ".png";

                using (var response = await client.GetAsync(model.Image))
                {

                    System.String imageName = string.Empty;

                    var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                    imageName = Path.GetRandomFileName() + extension;
                    using (var steam = System.IO.File.Create(Path.Combine(dirSave, imageName)))
                    {
                        await response.Content.CopyToAsync(steam);
                    }

                    var user = new UserEntity()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Image = imageName,
                        UserName = "user" + _userManager.Users.Count() + 1.ToString(),
                        Description = "",
                        Verified = false,
                        Country = model.Country,
                        CountryCode = model.CountryCode.ToLower(),
                        BackgroundImage = null,
                        AccessToken = model.Token
                    };
                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        var id = await _userManager.GetUserIdAsync(user);
                        user.UserName = "user" + id.ToString();
                        await _userManager.UpdateAsync(user);
                        result = await _userManager.AddToRoleAsync(user, Roles.User);
                        return Ok();
                    }
                }
                return BadRequest();
            }
            return BadRequest("User already exists");

        }

        // PUT api/<AuthController>/5
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest("Не вірно вказані дані");
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return BadRequest("Не вірно вказані дані");
                var token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("loginGoogle")]
        public async Task<IActionResult> LoginGoogle([FromForm] LoginGoogleViewModel model)
        {
            try
            {
                Console.Write("sdfsd");
                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
                GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(model.Token, settings).Result;

                if(payload == null)
                    return BadRequest("Login error!");

                var user = await _userManager.FindByEmailAsync(payload.Email);

                var token = "";

                if (user == null)
                {
                    var client = new HttpClient();
                    string fileName = null;
                    string extension = ".png";

                    using (var response = await client.GetAsync(payload.Picture))
                    {

                        System.String imageName = string.Empty;

                        var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                        imageName = Path.GetRandomFileName() + extension;
                        using (var steam = System.IO.File.Create(Path.Combine(dirSave, imageName)))
                        {
                            await response.Content.CopyToAsync(steam);
                        }

                        var newUser = new UserEntity()
                        {
                            Name = payload.Name,
                            Email = payload.Email,
                            Image = imageName,
                            UserName = "user" + _userManager.Users.Count() + 1.ToString(),
                            Description = "",
                            Verified = false,
                            Country = model.Country,
                            CountryCode = model.CountryCode.ToLower(),
                            BackgroundImage = null,
                            AccessToken = model.Token
                        };
                        var result = await _userManager.CreateAsync(newUser);

                        if (result.Succeeded)
                        {
                            var id = await _userManager.GetUserIdAsync(newUser);
                            newUser.UserName = "user" + id.ToString();
                            await _userManager.UpdateAsync(newUser);
                            result = await _userManager.AddToRoleAsync(newUser, Roles.User);
                            token = await _jwtTokenService.CreateToken(newUser);
                            return Ok(new { token });
                        }
                    }
                }

                token = await _jwtTokenService.CreateToken(user);
                return Ok(new {token});

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changeBackgroung")]
        public async Task<IActionResult> ChangeBG([FromForm] ChangeBgModel model)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email).Value;
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);
                System.String imageName = string.Empty;
                if (model.BackgroundImage != null)
                {
                    var fileExp = Path.GetExtension(model.BackgroundImage.FileName);
                    var dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
                    imageName = Path.GetRandomFileName() + fileExp;
                    using (var steam = System.IO.File.Create(Path.Combine(dirSave, imageName)))
                    {
                        await model.BackgroundImage.CopyToAsync(steam);
                    }

                    user.BackgroundImage = imageName;
                    await _userManager.UpdateAsync(user);  

                    return Ok();
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
