using System.Collections.Generic;
using Instagram.Enums;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Models
{
    public class User : IdentityUser
    {
        public string Login { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public Sex Sex { get; set; }
        public List<Post> Posts { get; set; }
        public List<User> FollowsUsers { get; set; }
        public List<User> FollowerUsers { get; set; }
    }
}