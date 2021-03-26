using Microsoft.EntityFrameworkCore;

namespace Instagram.Models
{
    public class InstagramContext : DbContext
    {
        public DbSet<User> Users;
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}