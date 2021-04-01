using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Instagram.Models;
using Instagram.Services;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Instagram.Controllers
{
    public class UsersController : Controller
    {
        private InstagramContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly FileUploadService _uploadService;
        private readonly IHostEnvironment _environment;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager,
            FileUploadService uploadService, IHostEnvironment environment, InstagramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uploadService = uploadService;
            _environment = environment;
            _db = db;
        }

        
        [HttpGet]
        public async Task<IActionResult> UserProfile(string id, string searchId)
        {
            string userId = id != null ? id : _userManager.GetUserId(User);
            
            if (searchId != null)
            {
                bool isSubscribed = false;
                List<Subscribe> subscribes = _db.Subscribes.ToList();
                foreach (var sub in subscribes)
                {
                    if (sub.SubscriberId == userId && sub.UserId == searchId)
                    {
                        isSubscribed = true;
                    }
                }
                ViewBag.IsSubscribed = isSubscribed ? "Отписаться" : "Подписаться";
                userId = searchId;
            }
            
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                ProfileViewModel model = new ProfileViewModel
                {
                    User = user,
                    Posts = _db.Posts.AsQueryable().Where(p => p.AuthorId == userId)
                        .Include(p => p.Author)
                        .OrderByDescending(p => p.CreationDate),
                    Subscribes = _db.Subscribes.AsQueryable().Where(s => s.SubscriberId == userId),
                    Followers = _db.Subscribes.AsQueryable().Where(s => s.UserId == userId)
                };
                ViewBag.UserPostsCount = model.Posts.Count();
                return View(model);
            }

            ViewBag.UserPostsCount = 0;
            return View(new ProfileViewModel());
        }
        
        
        [HttpGet]
        public IActionResult Searching(string search)
        {
            IQueryable<User> allUsers = _userManager.Users;
            List<User> resultUsers = new List<User>();
            
            if (search != null)
            {
                foreach (var user in allUsers)
                {
                    if(user.UserName.Contains(search) && !resultUsers.Contains(user)) resultUsers.Add(user);
                    if(user.Email.Contains(search) && !resultUsers.Contains(user)) resultUsers.Add(user);
                    if(user.Name.Contains(search) && !resultUsers.Contains(user)) resultUsers.Add(user);
                    if(user.Description.Contains(search) && !resultUsers.Contains(user)) resultUsers.Add(user);
                }

                return View(resultUsers);
            }

            return NotFound();
        }
        
        
        [HttpPost]
        public async Task<IActionResult> TrySubscribe(string userId) 
        {
            string subscriberId = _userManager.GetUserId(User);
            User subscriber = await _userManager.GetUserAsync(User);
            bool isSubscribed = false;

            if (userId != null)
            {
                User user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    List<Subscribe> subscribes = _db.Subscribes.ToList();
                    foreach (var subscribe in subscribes)
                    {
                        if (subscribe.SubscriberId == subscriberId && subscribe.UserId == userId)
                        {
                            _db.Entry(subscribe).State = EntityState.Deleted;
                            _db.SaveChanges();
                            isSubscribed = true;
                        }
                    }

                    ViewBag.IsSubscribed = isSubscribed ? "Отписаться" : "Подписаться";
                    if (!isSubscribed)
                    {
                        Subscribe subscribe = new Subscribe
                        {
                            UserId = user.Id,
                            User = user,
                            SubscriberId = subscriberId,
                            Subscriber = subscriber
                        };
                        _db.Subscribes.Add(subscribe);
                        _db.SaveChanges();
                    }
                }
                return RedirectToAction("UserProfile", "Users",  new {searchId = userId});
            }

            return NotFound();
        }
    }
}