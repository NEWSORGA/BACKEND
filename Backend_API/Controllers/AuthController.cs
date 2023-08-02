using AutoMapper;
using Backend_API.Asbtract;
using Backend_API.Constans;
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
using System.Drawing;
using System.Net;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        public AuthController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService, IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == id);

            if(user != null)
            {
                var res = _mapper.Map<UserViewModel>(user);

                return Ok(res);
            }
                
            return BadRequest("User not found!");
        }
        // POST api/<AuthController>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateUserViewModel model)
        {
            String imageName = string.Empty;
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

            String bgImageName = string.Empty;
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

                    String imageName = string.Empty;

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
                var us = GoogleJsonWebSignature.ValidateAsync(model.Token, new GoogleJsonWebSignature.ValidationSettings());
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest("Не вірно вказані дані");
                if (user.AccessToken != model.Token)
                    return BadRequest("Не вірно вказані дані");
                var token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });

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
                String imageName = string.Empty;
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
