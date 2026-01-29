using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pms.Dto.categoryDto;
using Pms.Service.Interface;
using Pms.Service.Service;
using System.Threading.Tasks;

namespace Pms.Server.Controllers
{
    [Route("api/categories")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Create")]

        public async Task<IActionResult> Create([FromForm] CategoryCreateDto categoryCreateDto)
        {
            await _categoryService.CreateAsync(categoryCreateDto);
            return Ok("Category created successfully");
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound("Category not found");
            return Ok(category);
        }

        [HttpPost("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound("Category not found or already inactive");
            return Ok("Category deleted successfully");
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(int id,[FromForm] CategoryUpdateDto categoryUpdateDto)
        {
            var result = await _categoryService.UpdateAsync(id,categoryUpdateDto);
            if (!result)
            {
                return NotFound("No category found with id:" + id);
            }
            return Ok("Category Updated successfully");
        }


    }
}
