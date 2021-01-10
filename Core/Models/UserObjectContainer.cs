namespace Core.Models
{
    public class UserObjectContainer
    {
        public User User { get; set; }

        public object Object { get; set; }

        public UserObjectContainer(User user, object obj)
        {
            User = user;
            Object = obj;
        }
    }
}
