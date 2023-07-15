using Backend_API.Data.Entities.Identity;

namespace Backend_API.Asbtract
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(UserEntity user);
    }
}
