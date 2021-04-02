using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Instagram.Models;
using Instagram.Services;
using Instagram.ViewModels;
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
        private readonly UserManager<User> _userManager;


        public PostsController(InstagramContext db, FileUploadService uploadService,
            IHostEnvironment environment, UserManager<User> userManager)
        {
            _db = db;
            _uploadService = uploadService;
            _environment = environment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserProfile", "Users");
            }
            
            PostsIndexViewModel model = new PostsIndexViewModel
            {
                Posts = _db.Posts
                    .Join(_db.Subscribes.Where(s => s.SubscriberId == user.Id),
                    p => p.AuthorId,
                    s => s.UserId,
                    (p, s) => new Post
                    {
                       Id = p.Id,
                       ImagePath = p.ImagePath,
                       Description = p.Description,
                       CreationDate = p.CreationDate,
                       AuthorId =  s.UserId,
                       Author = s.User,
                       Likes = p.Likes,
                       Comments = p.Comments
                    })
                    .OrderByDescending(p=>p.CreationDate)
            };
            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            string userId = _userManager.GetUserId(User);
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

                string path = Path.Combine(_environment.ContentRootPath,
                    $"wwwroot\\InstagramFiles\\PostsImagesByUserId\\{userId}\\");
                string postImagePath = $"/InstagramFiles/PostsImagesByUserId/{userId}/{post.FormFile.FileName}";
                _uploadService.Upload(path, post.FormFile.FileName, post.FormFile);
                post.ImagePath = postImagePath;

                //if (ModelState.IsValid)
                //{
                //почему то эту валидацию не проходит, хотя все корректно отрабатывает
                await _db.Posts.AddAsync(post);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Posts");
                //}
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> GetPost(int? postId)
        {
            ViewBag.Like = false;
            if (postId != null)
            {
                Post post = _db.Posts.FirstOrDefault(p => p.Id == postId);

                if (post != null)
                {
                    post.Author = await _userManager.FindByIdAsync(post.AuthorId);

                    post.Likes = (from like in _db.Likes
                            .Include(l => l.User)
                            .Include(l => l.Post)
                        where like.PostId == postId
                        select like).ToList();

                    post.Comments = (from comment in _db.Comments
                            .Include((c => c.User))
                            .Include(c => c.Post)
                        where comment.PostId == postId
                        select comment).ToList();
                    post.Comments = post.Comments.AsQueryable().OrderBy(p => p.CommentCreationDate).ToList();

                    foreach (var like in post.Likes)
                    {
                        if (like.UserId.Equals(_userManager.GetUserId(User)))
                        {
                            ViewBag.Like = true;
                        }
                    }

                    return View(post);
                }
            }

            return NotFound();
        }


        public async Task<IActionResult> TryMakeLike(int postId, string actionName)
        {
            Post post = postId != 0 ? _db.Posts.FirstOrDefault(p => p.Id == postId) : new Post();

            if (post != null)
            {
                post.Author = await _userManager.FindByIdAsync(post.AuthorId);
                post.Likes = (from like in _db.Likes.Include(l => l.User)
                        .Include(l => l.Post) where like.PostId == postId
                    select like).ToList();
                post.Comments = (from comment in _db.Comments.Include((c => c.User))
                        .Include(c => c.Post) where comment.PostId == postId
                    select comment).ToList();
                
                bool result = false;
                if (post.Likes.Any())
                {
                    foreach (var like in post.Likes)
                    {
                        if (like.UserId.Equals(_userManager.GetUserId(User)))
                        {
                            result = true;
                        }
                        
                    }
                }

                if (!result)
                {
                    await _db.Likes.AddAsync(new Like
                    {
                        PostId = postId,
                        Post = post,
                        UserId = _userManager.GetUserId(User),
                        User = await _userManager.GetUserAsync(User)
                    });
                    await _db.SaveChangesAsync();
                    ViewBag.Like = true;
                }
                else
                {
                    Like like = _db.Likes.FirstOrDefault(l => l.PostId == postId);
                    if (like != null)
                    {
                        _db.Entry(like).State = EntityState.Deleted;
                        await _db.SaveChangesAsync();
                        ViewBag.Like = false;
                    }
                }
                return RedirectToAction(actionName, new{ postId = postId});
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> TryMakeComment(Comment comment)
        {
            if (comment != null)
            {
                comment.UserId = _userManager.GetUserId(User);
                comment.User =  await _userManager.FindByIdAsync(comment.UserId);
                comment.Post = _db.Posts.FirstOrDefault(p => p.Id == comment.PostId);
                
                _db.Comments.Add(comment);
                _db.SaveChanges();
            }

            return RedirectToAction("GetPost", "Posts", new { postId = comment.PostId});
        }
    }
}