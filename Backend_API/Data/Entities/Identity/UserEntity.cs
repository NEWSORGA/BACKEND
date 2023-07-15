﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend_API.Data.Entities.Identity
{
    public class UserEntity : IdentityUser<int>
    {
        [StringLength(100)]
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool Verified { get; set; }
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
    }
}