namespace WebAPI.Model
{
    public class BasketPosition
    {
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public int Amount { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}
