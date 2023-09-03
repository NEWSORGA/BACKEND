using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend_API.Data.Entities.Identity;
using Backend_API.Data.Entities;

namespace Backend_API.Data
{
    public class AppEFContext : IdentityDbContext<UserEntity, RoleEntity, int,
        IdentityUserClaim<int>, UserRoleEntity, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
       
        public AppEFContext(DbContextOptions<AppEFContext> options)
            : base(options)
        {

        }

        public DbSet<TweetEntity> Tweets { get; set; }
        public DbSet<TweetMediaEnitity> TweetsMedias { get; set; }
        public DbSet<TweetLikeEntity> TweetsLikes { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<CommentMediaEntity> CommentsMedias { get; set; }
        public DbSet<FollowEntity> Follows { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserRoleEntity>(ur =>
            {
                ur.HasKey(ur => new { ur.UserId, ur.RoleId });

                ur.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(r => r.RoleId)
                    .IsRequired();

                ur.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired();
            });

        }
    }
}
