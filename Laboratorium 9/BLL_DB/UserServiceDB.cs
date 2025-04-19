using BLL.DTOModels.UserDTOs;
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
    public class UserServiceDB : IUserService
    {
        private readonly WebStoreContext _context;
        private static string? _userSession = null;
        private readonly string _connectionString;

        public UserServiceDB(WebStoreContext context)
        {
            _context = context;
            _connectionString = ("Data Source=(localdb)\\\\MSSQLLocalDB;Initial Catalog=DbSklep;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False\"");
        }

        public async Task<bool> Login(UserLoginRequestDTO dto)
        {
            using var conn = new SqlConnection(_connectionString);
            var success = await conn.ExecuteScalarAsync<int>("LoginUser", new { dto.Login, dto.Password }, commandType: CommandType.StoredProcedure);
            if (success > 0)
            {
                _userSession = dto.Login;
                return true;
            }
            return false;
        }

        public Task Logout()
        {
            _userSession = null;
            return Task.CompletedTask;
        }
    }
}
