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
        public string City { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Verified { get; set; }
    }
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
