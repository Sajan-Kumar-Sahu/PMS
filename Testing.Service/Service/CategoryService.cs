using Pms.Dto.categoryDto;
using Pms.Service.Interface;
using PmsRepository.Interface;
using PmsRepository.Models;
using Shared.Exceptions;
using System.Security.Claims;

namespace Pms.Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IImageService _imageService;
        
        public CategoryService(
            IGenericRepository<Category> repository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ICurrentUserContext currentUser,
            IImageService imageService)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _currentUser = currentUser;
            _imageService = imageService;
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

            string? imageUrl = null;
            if (categoryCreateDto.CategoryImage != null)
            {
                imageUrl = await _imageService.SaveImageAsync(
                    categoryCreateDto.CategoryImage,
                    "categories"
                );
            }

            var category = new Category
            {
                CategoryName = categoryCreateDto.CategoryName.Trim(),
                CategoryDescription = categoryCreateDto.CategoryDescription,
                CategoryImageUrl = imageUrl,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId,
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
            category.UpdatedDate = DateTime.UtcNow;
            category.UpdatedBy = _currentUser.UserId;

            _repository.Update(category);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            try
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
                        CategoryDescription = c.CategoryDescription,
                        CategoryImageUrl = c.CategoryImageUrl
                    })
                    .ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
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
                CategoryImageUrl = category.CategoryImageUrl,
                CreatedDate = category.CreatedDate
            };

        }

        public async Task<bool> UpdateAsync(int id,CategoryUpdateDto categoryUpdateDto)
        {
            if (string.IsNullOrWhiteSpace(categoryUpdateDto.CategoryName))
            {
                throw new InvalidOperationAppException("Category name cannot be empty.");
            }

            
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

            string? newImageUrl = null;
            string? oldImageUrl = existing.CategoryImageUrl;

            if (categoryUpdateDto.CategoryImage != null)
            {
                // Save new image (validated inside ImageService)
                newImageUrl = await _imageService.SaveImageAsync(
                    categoryUpdateDto.CategoryImage,
                    "categories"
                );

                existing.CategoryImageUrl = newImageUrl;
            }

            existing.CategoryName = newName;
            existing.CategoryDescription = categoryUpdateDto.CategoryDescription;
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = _currentUser.UserId;

            try
            {
                _repository.Update(existing);
                await _repository.SaveAsync();

                if (!string.IsNullOrEmpty(newImageUrl) &&
                    !string.IsNullOrEmpty(oldImageUrl))
                {
                    _imageService.DeleteImage(oldImageUrl);
                }

                return true;
            }
            catch
            {
                if (!string.IsNullOrEmpty(newImageUrl))
                    _imageService.DeleteImage(newImageUrl);

                throw;
            }
        }
    }
}
