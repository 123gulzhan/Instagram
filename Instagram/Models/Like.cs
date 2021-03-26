using System;

namespace Instagram.Models
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime LikeCreationDate { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int LikeUserId { get; set; }
        public User LikeUser { get; set; }
    }
}