using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pms.Dto.ProductDto;
using Pms.Service.Interface;
using PmsRepository.Models;

namespace Pms.Server.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto)
        {
            await _productService.CreateAsync(productCreateDto);
            return Ok("Product created successfully");

        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto productUpdateDto)
        {

            var updated = await _productService.UpdateAsync(id,productUpdateDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("next-sku")]
        public async Task<IActionResult> GetNextSku()
        {
            var sku = await _productService.GetNextSkuAsync();
            return Ok(sku);
        }
    }
}
