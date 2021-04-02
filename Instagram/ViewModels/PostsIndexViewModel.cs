using System.Linq;
using Instagram.Models;

namespace Instagram.ViewModels
{
    public class PostsIndexViewModel
    {
        public IQueryable<Post> Posts { get; set; }
    }
}