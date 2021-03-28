using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Instagram.Models;
using Instagram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Instagram.Controllers
{
    public class PostsController : Controller
    {
        private InstagramContext _db;
        private readonly FileUploadService _uploadService;
        private readonly IHostEnvironment _environment;
        private readonly UserManager<ClaimsPrincipal> _userManager;


        public PostsController(InstagramContext db, FileUploadService uploadService,
            IHostEnvironment environment, UserManager<ClaimsPrincipal> userManager)
        {
            _db = db;
            _uploadService = uploadService;
            _environment = environment;
            _userManager = userManager;
        }

        // GET
        public IActionResult Index()
        {
            IQueryable<Post> posts = _db.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.AuthorId);
            return View(posts);
        }


        [HttpGet]
        public IActionResult CreateAsync()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(Post post)
        {
            string userId = await _userManager.GetUserIdAsync(User);
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            
            if (userId != null && user != null && post != null)
            {
                post.AuthorId = userId;
                post.Author = user;
                post.Comments = new List<Comment>();
                post.Likes = new List<Like>();
                
                
                string dirName = "wwwroot/InstagramFiles/PostsImagesByUserId";
                DirectoryInfo dirInfo = new DirectoryInfo(dirName);
                string[] dirs = Directory.GetDirectories(dirName);
                if (!dirs.AsQueryable().Contains(userId))
                {
                    dirInfo.CreateSubdirectory(userId);
                }

                
                string path = Path.Combine(_environment.ContentRootPath, $"wwwroot\\InstagramFiles\\PostsImagesByUserId\\{userId}\\");
                string postImagePath = $"/InstagramFiles/PostsImagesByUserId/{userId}/{post.FormFile.FileName}";
                _uploadService.Upload(path, post.FormFile.FileName, post.FormFile);
                post.ImagePath = postImagePath;

                if (ModelState.IsValid)
                {
                    await _db.Posts.AddAsync(post);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Posts");
                }
            }

            return NotFound();

        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}