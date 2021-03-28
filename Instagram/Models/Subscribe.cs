namespace Instagram.Models
{
    public class Subscribe
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string SubscriberId{ get; set; }
        public User Subscriber { get; set; }
    }
}