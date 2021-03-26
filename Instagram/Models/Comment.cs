using System;

namespace Instagram.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CommentCreationDate { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int CommentUserId { get; set; }
        public User CommentUser { get; set; }
        public string CommentText { get; set; }
    }
}