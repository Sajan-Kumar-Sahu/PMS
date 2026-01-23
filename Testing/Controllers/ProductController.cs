using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pms.Dto.ProductDto;
using Pms.Service.Interface;

namespace Pms.Server.Controllers
{
    [Route("api/products")]
    [Authorize]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetProductsByCategory(int id)
        {
            var products = await _productService
                .GetByCategoryIdAsync(id);

            return Ok(products);
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

            return Ok("Product Updated Successfully");
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return Ok("Product Deleted Successfully");
        }

        [HttpGet("next-sku")]
        public async Task<IActionResult> GetNextSku()
        {
            var sku = await _productService.GetNextSkuAsync();
            return Ok(sku);
        }

    }
}
