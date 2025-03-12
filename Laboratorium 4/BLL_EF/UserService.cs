using BLL.DTOModels.UserDTOs;
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
    public class UserService : IUserService
    {
        private readonly WebStoreContext _context;
        static bool logged = true; 

        public UserService(WebStoreContext context)
        {
            _context = context;
        }

        // Logowanie użytkownika
        public async Task<bool> Login(UserLoginRequestDTO userLoginRequestDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == userLoginRequestDTO.Login && u.Password == userLoginRequestDTO.Password && u.IsActive);

            if (user != null)
            {
                logged = true;
                return true;
            }
            else return false;
        }

        // Wylogowanie użytkownika
        public async Task Logout()
        {
            logged = false;
            await Task.CompletedTask;
        }
    }
}
