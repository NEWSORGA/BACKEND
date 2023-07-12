using Microsoft.AspNetCore.Identity;

namespace Backend_API.Data.Entities.Identity
{
    public class RoleEntity : IdentityRole<int>
    {
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
    }
}
