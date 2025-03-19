using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOModels.OrderDTOs;

namespace BLL.ServiceInterfaces
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderResponseDTO>> GetOrder(int? idFilter, bool? paidFilter, string? sortBy,
            bool sortOrder);

        public Task<OrderDetailsResponseDTO> GetOrderDetails(int orderID);
        public Task PayOrder(int orderID, double amount);
    }
}
