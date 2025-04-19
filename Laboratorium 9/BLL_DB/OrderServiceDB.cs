using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DB
{
    public class OrderServiceDB : IOrderService
    {
        private readonly WebStoreContext _context;
        private readonly string _connectionString;

        public async Task<IEnumerable<OrderResponseDTO>> GetOrder(int? idFilter, bool? paidFilter, string? sortBy, bool sortOrder)
        {
            using var conn = new SqlConnection(_connectionString);

            var sql = @"SELECT o.ID AS OrderID, o.Date AS OrderDate, SUM(op.Price * op.Amount) AS TotalPrice, o.IsPaid
                    FROM Orders o
                    JOIN OrderPositions op ON o.ID = op.OrderID
                    WHERE (@idFilter IS NULL OR o.ID = @idFilter)
                      AND (@paidFilter IS NULL OR o.IsPaid = @paidFilter)
                    GROUP BY o.ID, o.Date, o.IsPaid";

            if (!string.IsNullOrEmpty(sortBy))
                sql += $" ORDER BY {sortBy} {(sortOrder ? "ASC" : "DESC")}";

            return await conn.QueryAsync<OrderResponseDTO>(sql, new { idFilter, paidFilter });
        }

        public async Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);

            var sql = @"SELECT p.Name AS ProductName, op.Price, op.Amount, (op.Price * op.Amount) AS TotalValue
                    FROM OrderPositions op
                    JOIN Products p ON op.ProductID = p.ID
                    WHERE op.OrderID = @orderId";

            var items = await conn.QueryAsync<OrderItemDTO>(sql, new { orderId });

            return new OrderDetailsResponseDTO { OrderPositions = items.ToList() };
        }

        public async Task PayOrder(int orderId, double amount)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("PayOrder", new { OrderID = orderId, Amount = amount }, commandType: CommandType.StoredProcedure);
        }
    }
}
