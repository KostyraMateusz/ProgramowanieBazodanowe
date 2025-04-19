using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.OrderDTOs
{
    public class OrderDetailsResponseDTO
    {
        public List<OrderItemDTO> OrderPositions { get; init; }
    }
}
