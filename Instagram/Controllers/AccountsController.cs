using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Instagram.Enums;
using Instagram.Models;
using Instagram.Services;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace Instagram.Controllers
{
    public class AccountsController : Controller
    {
        private InstagramContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        private readonly FileUploadService _uploadService;
        private readonly IHostEnvironment _environment;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, 
            FileUploadService uploadService, IHostEnvironment environment, InstagramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uploadService = uploadService;
            _environment = environment;
            _db = db;
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            List<string> sexType = Enum.GetNames(typeof(Sex)).ToList();
            ViewBag.SexType = sexType;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            string path = Path.Combine(_environment.ContentRootPath, "wwwroot\\InstagramFiles\\Avatars\\");
            string avatarPath = $"/InstagramFiles/Avatars/{model.FormFile.FileName}";
            _uploadService.Upload(path, model.FormFile.FileName, model.FormFile);
            model.Avatar = avatarPath;
            
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.Login,
                    Email = model.Email,
                    Avatar = model.Avatar,
                    Name = model.Name,
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Sex == Sex.Female ? Sex.Female
                    : model.Sex == Sex.Male ? Sex.Male
                    : Sex.NotSelected//,
                    //Posts = new List<Post>()
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("UserProfile", "Accounts");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(model.LoginOrEmail);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(model.LoginOrEmail);
                }

                SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe, false);

                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("UserProfile", "Accounts");
                }
                ModelState.AddModelError("", "Неправильный логин и(или) пароль");
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("UserProfile", "Accounts");
        }

        
        [HttpGet]
        
        public async Task<IActionResult> UserProfile()
        {
            string userId = _userManager.GetUserId(User);
            
            User user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                 IQueryable<Post> posts = _db.Posts.AsQueryable().Where(p => p.AuthorId == userId)
                                .Include(p=>p.Author);
                            
                            ProfileViewModel model = new ProfileViewModel
                            {
                                User = user,
                                Posts = posts,
                                Subscribes = _db.Subscribes.AsQueryable().Where(s => s.SubscriberId == userId),
                                Followers = _db.Subscribes.AsQueryable().Where(s => s.UserId == userId)
                            };
                            return View(model);
            }

            return View(new ProfileViewModel());
        }
        
        
        
    }
}