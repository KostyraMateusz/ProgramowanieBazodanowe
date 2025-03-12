using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOModels.UserDTOs;

namespace BLL.ServiceInterfaces
{
    public interface IUserService
    {
        public Task<bool> Login(UserLoginRequestDTO userLoginRequestDTO);
        public Task Logout();
    }
}
