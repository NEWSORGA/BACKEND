namespace Backend_API.Models.Auth
{
    public class CreateUserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile BackgroundImage { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }
    public class CreateGoogleUserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Token { get; set; }
    }


    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string BackgroundImage { get; set; }
        public string Description { get; set; }
        public string Verified { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginGoogleViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ChangeBgModel
    {
        public IFormFile BackgroundImage { get; set; }
    }
}
