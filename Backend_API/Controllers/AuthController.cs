using Backend_API.Asbtract;
using Backend_API.Constans;
using Backend_API.Data.Entities.Identity;
using Backend_API.Models.Auth;
using Backend_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        public AuthController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
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
     
          
            var user = new UserEntity()
            {
                Name = model.Name,
                Email = model.Email,
                Image = imageName,
                UserName = "user" + _userManager.Users.Count()+1.ToString(),
                Description = "",
                Verified = false,
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

        // PUT api/<AuthController>/5
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
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

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
