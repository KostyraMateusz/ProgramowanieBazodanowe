namespace WebAPI.Model
{
    public class UserGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
