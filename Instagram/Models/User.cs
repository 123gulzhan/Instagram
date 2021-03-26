using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Instagram.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sex Sex { get; set; }
        public List<Post> Posts { get; set; }
       
    }
}