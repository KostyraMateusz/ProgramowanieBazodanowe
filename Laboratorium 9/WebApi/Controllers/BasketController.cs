using BLL.DTOModels.BasketDTOs;
using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpPost("AddProductToBasket")]
        public async Task<IActionResult> AddProductToBasket([FromBody] BasketRequestDTO basketDto)
        {
            await _basketService.AddProductToBasket(basketDto);
            return Ok();
        }

        [HttpPost("CreateOrder/{userId}")]
        public async Task<IActionResult> CreateOrder(int userId)
        {
            var order = await _basketService.CreateOrder(userId);
            return Ok(order);
        }

        [HttpDelete("RemoveFromBasket/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromBasket(int userId, int productId)
        {
            await _basketService.RemoveFromBasket(userId, productId);
            return Ok();
        }

        [HttpPut("UpdateBasketItem")]
        public async Task<IActionResult> UpdateBasketItem(int userId, int productId, int amount)
        {
            await _basketService.UpdateBasketItem(userId, productId, amount);
            return Ok();
        }
    }
}
