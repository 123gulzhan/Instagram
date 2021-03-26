using Instagram.Models;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class PostsController : Controller
    {
        private InstagramContext _db;

        public PostsController(InstagramContext db)
        {
            _db = db;
        }
        
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}