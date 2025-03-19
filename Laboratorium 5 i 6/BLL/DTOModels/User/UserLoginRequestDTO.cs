using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.UserDTOs
{
    public class UserLoginRequestDTO
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
