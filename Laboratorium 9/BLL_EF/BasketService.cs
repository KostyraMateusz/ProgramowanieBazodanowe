using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOModels.BasketDTOs;
using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using DAL;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BLL_EF.Services
{
    public class BasketService : IBasketService
    {
        private readonly WebStoreContext _context;

        public BasketService(WebStoreContext context)
        {
            _context = context;
        }

        public async Task AddProductToBasket(BasketRequestDTO basketDto)
        {
            var basketItem = await _context.BasketPositions
                .FirstOrDefaultAsync(bp => bp.ProductID == basketDto.ProductID && bp.UserID == basketDto.UserID);

            if (basketItem == null)
            {
                basketItem = new BasketPosition
                {
                    ProductID = basketDto.ProductID,
                    UserID = basketDto.UserID,
                    Amount = basketDto.Amount
                };
                _context.BasketPositions.Add(basketItem);
            }
            else
            {
                basketItem.Amount += basketDto.Amount;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<OrderResponseDTO> CreateOrder(int userId)
        {
            var basketItems = await _context.BasketPositions
                .Include(bp => bp.Product)
                .Where(bp => bp.UserID == userId)
                .ToListAsync();

            if (!basketItems.Any())
                throw new InvalidOperationException("Basket is empty.");

            var order = new Order
            {
                UserID = userId,
                Date = DateTime.Now,
                OrderPositions = basketItems.Select(bp => new OrderPosition
                {
                    ProductId = bp.ProductID,
                    Amount = bp.Amount,
                    Price = bp.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.BasketPositions.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            return new OrderResponseDTO
            {
                OrderID = order.ID,
                OrderDate = order.Date,
                TotalPrice = order.OrderPositions.Sum(op => op.Price * op.Amount),
                IsPaid = order.IsPaid
            };
        }

        public async Task RemoveFromBasket(int userId, int productId)
        {
            var basketItem = await _context.BasketPositions
                .FirstOrDefaultAsync(bp => bp.UserID == userId && bp.ProductID == productId);

            if (basketItem != null)
            {
                _context.BasketPositions.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateBasketItem(int userId, int productId, int amount)
        {
            var basketItem = await _context.BasketPositions
                .FirstOrDefaultAsync(bp => bp.UserID == userId && bp.ProductID == productId);

            if (basketItem != null)
            {
                basketItem.Amount = amount;
                await _context.SaveChangesAsync();
            }
        }

    }
}