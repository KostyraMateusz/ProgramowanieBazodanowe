namespace WebAPI.Model
{
    public class Order
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public bool IsPaid { get; set; }
        public DateTime Date { get; set; }
        public User? User { get; set; }
        public ICollection<OrderPosition>? OrderPositions { get; set; }
    }
}
