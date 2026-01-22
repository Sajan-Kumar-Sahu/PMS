using Pms.Dto.categoryDto;
using Pms.Service.Interface;
using PmsRepository.Interface;
using PmsRepository.Models;
using Shared.Exceptions;

namespace Pms.Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        public CategoryService(IGenericRepository<Category> repository, ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        public async Task CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            if (string.IsNullOrWhiteSpace(categoryCreateDto.CategoryName))
                throw new InvalidOperationAppException("Category name cannot be empty.");

            bool categoryExists = await _categoryRepository.ExistsAsync(c =>
                c.CategoryName.ToLower() == categoryCreateDto.CategoryName.Trim().ToLower()
                && c.IsActive);

            if (categoryExists)
                throw new AlreadyExistsException("Category already exists.");

            var category = new Category
            {
                CategoryName = categoryCreateDto.CategoryName.Trim(),
                CategoryDescription = categoryCreateDto.CategoryDescription,
                CreatedDate = DateTime.Now,
                CreatedBy = 1,
                UpdatedBy = 0,
                IsActive = true
            };

            await _repository.AddAsync(category);
            await _repository.SaveAsync();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null || !category.IsActive)
            {
                throw new NotFoundException("Category does not exist.");
            }

            bool hasProducts = await _productRepository.ExistsAsync(p =>
                p.CategoryId == id && p.IsActive);

            if (hasProducts)
            {
                throw new InvalidOperationAppException(
                    "Category cannot be deleted because products are associated with it."
                );
            }

            category.IsActive = false;
            category.UpdatedDate = DateTime.Now;
            category.UpdatedBy = 1;

            _repository.Update(category);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            if (categories == null)
            {
                throw new InvalidOperationAppException(
                    "Failed to retrieve categories."
                );
            }

            return categories
                .Where(c => c.IsActive)   // important if you use soft delete
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription
                })
                .ToList();
        }

        public async Task<CategoryDetailsDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                throw new NotFoundException("Category does not exist.");
            }

            if (!category.IsActive)
            {
                throw new NotFoundException("Category does not exist.");
            }

            return new CategoryDetailsDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                CreatedDate = category.CreatedDate
            };

        }

        public async Task<bool> UpdateAsync(int id,CategoryUpdateDto categoryUpdateDto)
        {
            if (string.IsNullOrWhiteSpace(categoryUpdateDto.CategoryName))
            {
                throw new InvalidOperationAppException("Category name cannot be empty.");
            }

            // 2️⃣ Fetch existing category
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null || !existing.IsActive)
            {
                throw new NotFoundException("Category does not exist.");
            }

            var newName = categoryUpdateDto.CategoryName.Trim();

            bool duplicateExists = await _categoryRepository.ExistsAsync(c =>
                c.CategoryId != existing.CategoryId &&
                c.CategoryName.ToLower() == newName.ToLower() &&
                c.IsActive);

            if (duplicateExists)
            {
                throw new AlreadyExistsException(
                    $"Category '{newName}' already exists."
                );
            }

            existing.CategoryName = newName;
            existing.CategoryDescription = categoryUpdateDto.CategoryDescription;
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = 1;

            _repository.Update(existing);
            await _repository.SaveAsync();

            return true;
        }
    }
}
