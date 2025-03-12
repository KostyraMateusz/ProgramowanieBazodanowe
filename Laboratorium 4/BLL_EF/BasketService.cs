using BLL.DTOModels.BasketDTOs;
using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;

namespace BLL_EF
{
    public class BasketService : IBasketService
    {
        private readonly WebStoreContext _context;

        public BasketService(WebStoreContext context)
        {
            _context = context;
        }

        // Dodanie produktu do koszyka
        public async Task AddProductToBasket(BasketRequestDTO basketRequestDto)
        {
            var basketPosition = await _context.BasketPositions
                .FirstOrDefaultAsync(b => b.UserID == basketRequestDto.UserID && b.ProductID == basketRequestDto.ProductID);

            if (basketPosition != null)
            {
                // Jeśli produkt już istnieje w koszyku to zwiększamy ilość
                basketPosition.Amount += basketRequestDto.Amount;
                _context.BasketPositions.Update(basketPosition);
            }
            else
            {
                // Jeśli produkt nie istnieje w koszyku to go dodajemy
                var newBasketPosition = new BasketPosition
                {
                    UserID = basketRequestDto.UserID,
                    ProductID = basketRequestDto.ProductID,
                    Amount = basketRequestDto.Amount
                };
                await _context.BasketPositions.AddAsync(newBasketPosition);
            }

            await _context.SaveChangesAsync();
        }

        // Zaktualizowanie ilości produktu w koszyku
        public async Task UpdateBasketItemAsync(int userId, int productId, int amount)
        {
            var basketItem = await _context.BasketPositions
                .FirstOrDefaultAsync(b => b.UserID == userId && b.ProductID == productId);

            if (basketItem != null)
            {
                basketItem.Amount = amount;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found in basket.");
            }
        }

        // Usunięcie produktu z koszyka
        public async Task RemoveFromBasketAsync(int userId, int productId)
        {
            var basketItem = await _context.BasketPositions
                .FirstOrDefaultAsync(b => b.UserID == userId && b.ProductID == productId);

            if (basketItem != null)
            {
                _context.BasketPositions.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found in basket.");
            }
        }

        // Tworzenie zamówienia na podstawie produktów w koszyku
        public async Task<OrderResponseDTO> CreateOrderAsync(int userId)
        {
            var basketItems = await _context.BasketPositions
                .Where(b => b.UserID == userId)
                .ToListAsync();

            if (!basketItems.Any())
                throw new Exception("No items in basket.");

            double totalPrice = 0;
            List<OrderPosition> orderPositions = new List<OrderPosition>();

            foreach (var item in basketItems)
            {
                var product = await _context.Products.FindAsync(item.ProductID);
                if (product != null)
                {
                    totalPrice += product.Price * item.Amount;

                    var orderPosition = new OrderPosition
                    {
                        ProductID = product.ID,
                        Amount = item.Amount,
                        Price = product.Price
                    };
                    orderPositions.Add(orderPosition);
                }
            }

            var order = new Order
            {
                UserID = userId,
                Date = DateTime.Now,
                OrderPositions = orderPositions
            };

            await _context.Orders.AddAsync(order);

            _context.BasketPositions.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            return new OrderResponseDTO
            {
                OrderID = order.ID,
                TotalPrice = totalPrice,
                IsPaid = false,
                OrderDate = order.Date
            };
        }
    }

}
