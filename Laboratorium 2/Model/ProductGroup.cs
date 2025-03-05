namespace WebAPI.Model
{
    public class ProductGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }
        public ProductGroup? ParentGroup { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
