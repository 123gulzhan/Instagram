using System;

namespace Instagram.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CommentCreationDate { get; set; } = DateTime.Now;
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string CommentText { get; set; }
    }
}