using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Instagram.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int PostAuthorId { get; set; }
        public User PostAuthor { get; set; }
        public string PostImagePath { get; set; }
        public FormFile FormFile { get; set; }
        public string PostDescription { get; set; }
        public DateTime PostCreationDate { get; set; }
        public List<Like> PostLikes { get; set; }
        public List<Comment> PostComments { get; set; }
        
    }
}