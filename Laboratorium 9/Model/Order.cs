using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Order
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public bool IsPaid { get; set; }

        public User User { get; set; } = null!;
        public ICollection<OrderPosition> OrderPositions { get; set; } = new List<OrderPosition>();
    }
}
