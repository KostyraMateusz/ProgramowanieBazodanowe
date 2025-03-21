using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }

        public ProductGroup? Parent { get; set; }
        public ICollection<ProductGroup> SubGroups { get; set; } 
        public ICollection<Product> Products { get; set; }
    }
}
