using System.Collections.Generic;
using System.Linq;
using Instagram.Models;

namespace Instagram.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IQueryable<Post> Posts { get; set; }
        public IQueryable<Subscribe> Subscribes { get; set; }
        public IQueryable<Subscribe> Followers { get; set; }
    }
}