using BLL.DTOModels.BasketDTOs;
using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DB
{
    public class BasketServiceDB : IBasketService
    {
        private readonly WebStoreContext _context;
        private readonly string _connectionString;

        public BasketServiceDB(WebStoreContext context)
        {
            _context = context;
            _connectionString = ("Data Source=(localdb)\\\\MSSQLLocalDB;Initial Catalog=DbSklep;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False\"");
        }

        public async Task AddProductToBasket(BasketRequestDTO dto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("AddProductToBasket", new
            {
                UserId = dto.UserID,
                ProductId = dto.ProductID,
                Amount = dto.Amount
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateBasketItem(int userId, int productId, int amount)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("UpdateBasketItem", new { UserId = userId, ProductId = productId, Amount = amount }, commandType: CommandType.StoredProcedure);
        }

        public async Task RemoveFromBasket(int userId, int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("RemoveFromBasket", new { UserId = userId, ProductId = productId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<OrderResponseDTO> CreateOrder(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QuerySingleAsync<OrderResponseDTO>("CreateOrder", new { UserId = userId }, commandType: CommandType.StoredProcedure);
        }
    }
}
