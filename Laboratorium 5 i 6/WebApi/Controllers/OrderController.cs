using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder(int? idFilter, bool? paidFilter, string? sortBy, bool sortOrder)
        {
            var orders = await _orderService.GetOrder(idFilter, paidFilter, sortBy, sortOrder);
            return Ok(orders);
        }

        [HttpGet("GetOrderDetails/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetails(orderId);
            return Ok(orderDetails);
        }

        [HttpPost("PayOrder/{orderId}")]
        public async Task<IActionResult> PayOrder(int orderId, [FromBody] double amount)
        {
            await _orderService.PayOrder(orderId, amount);
            return Ok();
        }
    }
}
