using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Instagram.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public User Author { get; set; }
        [Required]
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public string Description { get; set; }
        
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [Required]
        public List<Like> Likes { get; set; }
        [Required]
        public List<Comment> Comments { get; set; }
        
    }
}