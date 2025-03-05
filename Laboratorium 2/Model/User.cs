namespace WebAPI.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int? GroupID { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
