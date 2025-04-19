using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;
using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter,
            string? sortBy, bool sortOrder, bool includeInactive)
        {
            var products = await _productService.GetProducts(nameFilter, groupNameFilter, groupIdFilter, sortBy, sortOrder, includeInactive);
            return Ok(products);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductRequestDTO productDto)
        {
            await _productService.AddProduct(productDto);
            return Ok();
        }

        [HttpPut("ChangeProductStatus/{productId}")]
        public async Task<IActionResult> ChangeProductStatus(int productId)
        {
            await _productService.ChangeProductStatus(productId);
            return Ok();
        }

        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProduct(productId);
            return Ok();
        }

        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups(int? parentId, string? sortBy, bool sortOrder)
        {
            var groups = await _productService.GetGroups(parentId, sortBy, sortOrder);
            return Ok(groups);
        }

        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup([FromBody] GroupRequestDTO groupRequestDTO)
        {
            await _productService.AddGroup(groupRequestDTO);
            return Ok();
        }
    }
}