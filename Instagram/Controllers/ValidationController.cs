using System.Linq;
using Instagram.Models;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class ValidationController : Controller
    {
        private InstagramContext _db;

        public ValidationController(InstagramContext db)
        {
            _db = db;
        }

        [HttpGet]
        public bool CheckLoginOrEmail(string loginOrEmail)
        {
            return _db.Users.Any(u => u.UserName.ToLower() == loginOrEmail.ToLower())
                   || _db.Users.Any(u => u.Email.ToLower() == loginOrEmail.ToLower());
        }
        
        [HttpGet]
        public bool CheckLogin(string login)
        {
            return !_db.Users.Any(u => u.UserName.ToLower() == login.ToLower());
        }
        
        [HttpGet]
        public bool CheckEmail(string email)
        {
            return !_db.Users.Any(u => u.Email.ToLower() == email.ToLower());
        }
    }
}