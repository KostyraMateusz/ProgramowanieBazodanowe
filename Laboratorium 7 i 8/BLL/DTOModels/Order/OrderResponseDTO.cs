using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.OrderDTOs
{
    public class OrderResponseDTO
    {
        public int OrderID { get; init; }
        public double TotalPrice { get; init; }
        public bool IsPaid { get; init; }
        public DateTime OrderDate { get; init; }
    }
}