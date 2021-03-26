using Instagram.Models;
using Instagram.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Instagram.Controllers
{
    public class PostsController : Controller
    {
        private InstagramContext _db;
        private readonly FileUploadService _uploadService;
        private readonly IHostEnvironment _environment;

        public PostsController(InstagramContext db, FileUploadService uploadService, IHostEnvironment environment)
        {
            _db = db;
            _uploadService = uploadService;
            _environment = environment;
        }
        
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}