using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOModels.BasketDTOs;
using BLL.DTOModels.OrderDTOs;

namespace BLL.ServiceInterfaces
{
    public interface IBasketService
    {
        public Task AddProductToBasket(BasketRequestDTO basketRequestDto);
        public Task UpdateBasketItemAsync(int userId, int productId, int amount);
        public  Task RemoveFromBasketAsync(int userId, int productId);
        public Task<OrderResponseDTO> CreateOrderAsync(int userId);
    }
}