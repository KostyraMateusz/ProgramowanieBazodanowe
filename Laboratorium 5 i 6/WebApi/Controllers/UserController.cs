using BLL.DTOModels.UserDTOs;
using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userLoginRequestDTO)
        {
            bool result = await _userService.Login(userLoginRequestDTO);
            if (result)
                return Ok();
            return Unauthorized();
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _userService.Logout();
            return Ok();
        }
    }
}
