using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_EF
{
    public class OrderService : IOrderService
    {
        private readonly WebStoreContext _context;

        public OrderService(WebStoreContext context)
        {
            _context = context;
        }

        // Pobranie zamówień na podstawie filtrów
        public async Task<IEnumerable<OrderResponseDTO>> GetOrder(int? idFilter, bool? paidFilter, string? sortBy, bool sortOrder)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            if (idFilter.HasValue)
                ordersQuery = ordersQuery.Where(o => o.ID == idFilter.Value);

            if (paidFilter.HasValue)
                ordersQuery = ordersQuery.Where(o => o.IsPaid == paidFilter.Value);

            // Sortowanie wyników
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "date")
                {
                    ordersQuery = sortOrder ? ordersQuery.OrderBy(o => o.Date) : ordersQuery.OrderByDescending(o => o.Date);
                }
                else if (sortBy.ToLower() == "price")
                {
                    ordersQuery = sortOrder ? ordersQuery.OrderBy(o => o.OrderPositions.Sum(op => op.Price * op.Amount)) :
                                               ordersQuery.OrderByDescending(o => o.OrderPositions.Sum(op => op.Price * op.Amount));
                }
            }

            var orders = await ordersQuery
                .Include(o => o.OrderPositions)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.ID,
                TotalPrice = o.OrderPositions.Sum(op => op.Price * op.Amount),
                IsPaid = o.IsPaid,
                OrderDate = o.Date
            });
        }

        // Pobranie szczegółów zamówienia
        public async Task<OrderDetailsResponseDTO> GetOrderDetails(int orderID)
        {
            var order = await _context.Orders
                .Where(o => o.ID == orderID)
                .Include(o => o.OrderPositions)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("Order not found");

            return new OrderDetailsResponseDTO
            {
                ProductName = order.OrderPositions.FirstOrDefault()?.Product?.Name,
                Price = order.OrderPositions.FirstOrDefault()?.Price ?? 0,
                Amount = order.OrderPositions.FirstOrDefault()?.Amount ?? 0,
                Balance = (order.OrderPositions.FirstOrDefault()?.Amount ?? 0) * (order.OrderPositions.FirstOrDefault()?.Price ?? 0)
            };
        }

        // Dokonanie płatności za zamówienie
        public async Task PayOrder(int orderID, double amount)
        {
            var order = await _context.Orders.FindAsync(orderID);

            if (order == null)
                throw new Exception("Order not found");

            if (order.IsPaid)
                throw new Exception("Order has already been paid");

            order.IsPaid = true;

            await _context.SaveChangesAsync();
        }
    }
}
