﻿using System;
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
                    UserName = model.Login.ToLower(),
                    Email = model.Email,
                    Avatar = model.Avatar,
                    Name = model.Name,
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Sex == Sex.Female ? Sex.Female
                        : model.Sex == Sex.Male ? Sex.Male
                        : Sex.NotSelected
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
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
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
                return RedirectToAction("UserProfile", new {searchId = userId});
            }

            return NotFound();
        }
    }
}