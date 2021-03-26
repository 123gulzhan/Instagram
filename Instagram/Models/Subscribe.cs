namespace Instagram.Models
{
    public class Subscribe
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SubscriberId{ get; set; }
        public User Subscriber { get; set; }
    }
}