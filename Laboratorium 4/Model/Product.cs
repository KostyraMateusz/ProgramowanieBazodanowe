namespace WebAPI.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public int? GroupID { get; set; }

        public ProductGroup? ProductGroup { get; set; }
    }
}
