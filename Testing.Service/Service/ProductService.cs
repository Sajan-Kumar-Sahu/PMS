using Pms.Dto.categoryDto;
using Pms.Dto.ProductDto;
using Pms.Dto.ProductDto.ProductDto;
using Pms.Service.Interface;
using PmsRepository.Interface;
using PmsRepository.Models;
using Shared.Exceptions;

namespace Pms.Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IImageService _imageService;

        public ProductService(
            IGenericRepository<Product> productRepo,
            IGenericRepository<Category> categoryRepo,
            IProductRepository productRepository,
            ICurrentUserContext currentUser,
            IImageService imageService)
        {
            _productRepo = productRepo;
            _productRepository = productRepository;
            _categoryRepo = categoryRepo;
            _currentUser = currentUser;
            _imageService = imageService;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null || !product.IsActive)
            {
                throw new NotFoundException("Product does not exist.");
            }

            product.IsActive = false;
            product.UpdatedDate = DateTime.UtcNow;
            product.UpdatedBy = _currentUser.UserId;

            _productRepo.Update(product);
            await _productRepo.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();

            if (products == null)
            {
                throw new InvalidOperationAppException(
                    "Failed to retrieve products."
                );
            }

            if (categories == null)
            {
                throw new InvalidOperationAppException(
                    "Failed to retrieve categories."
                );
            }

            var categoryLookup = categories
                .Where(c => c.IsActive)
                .ToDictionary(c => c.CategoryId, c => c.CategoryName);

            return products
                .Where(p => p.IsActive).OrderByDescending(p=>p.UpdatedDate ?? p.CreatedDate)
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = categoryLookup.TryGetValue(
                        p.CategoryId, out var categoryName)
                            ? categoryName
                            : "N/A",
                    Price = p.Price,
                    ProductImageUrl = p.ProductImageUrl
                })
                .ToList();
        }

        public async Task<ProductDetailsDto> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null || !product.IsActive)
            {
                throw new NotFoundException("Product does not exist.");
            }

            var categories = await _categoryRepo.GetAllAsync();

            if (categories == null)
            {
                throw new InvalidOperationAppException(
                    "Failed to retrieve categories."
                );
            }

            var category = categories
                .FirstOrDefault(c =>
                    c.CategoryId == product.CategoryId &&
                    c.IsActive);

            return new ProductDetailsDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                ProductDescription = product.ProductDescription,
                ProductImageUrl = product.ProductImageUrl,
                Sku = product.Sku,
                CategoryName = category?.CategoryName ?? "N/A",
                CreatedDate = product.CreatedDate
            };
        }

        public async Task CreateAsync(ProductCreateDto productCreateDto)
        {
            if (string.IsNullOrWhiteSpace(productCreateDto.ProductName))
            {
                throw new InvalidOperationAppException(
                    "Product name cannot be empty."
                );
            }

            if (productCreateDto.Price <= 0)
            {
                throw new InvalidOperationAppException(
                    "Product price must be greater than zero."
                );
            }

            // 2️⃣ Validate category
            var category = await _categoryRepo.GetByIdAsync(
                productCreateDto.CategoryId);

            if (category == null || !category.IsActive)
            {
                throw new NotFoundException("Invalid category.");
            }

            // 3️⃣ Check duplicate product (same name + category)
            bool productExists = await _productRepository.ExistsAsync(p =>
                p.ProductName.ToLower() ==
                    productCreateDto.ProductName.Trim().ToLower()
                && p.CategoryId == productCreateDto.CategoryId
                && p.IsActive);

            if (productExists)
            {
                throw new AlreadyExistsException(
                    "Product already exists in this category."
                );
            }

            var lastSku = await _productRepo.GetLastSkuAsync();
            var newSku = SkuGenerator.GenerateNextSku(lastSku);

            string? imageUrl = null;
            if (productCreateDto.ProductImage != null)
            {
                imageUrl = await _imageService.SaveImageAsync(
                    productCreateDto.ProductImage,
                    "products"
                );
            }
            var product = new Product
            {
                Sku = newSku,
                ProductName = productCreateDto.ProductName.Trim(),
                CategoryId = productCreateDto.CategoryId,
                ProductDescription = productCreateDto.ProductDescription,
                ProductImageUrl = imageUrl,
                Price = productCreateDto.Price,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId,
                IsActive = true
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveAsync();
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new InvalidOperationAppException(
                    "Product name cannot be empty."
                );
            }

            if (product.Price <= 0)
            {
                throw new InvalidOperationAppException(
                    "Product price must be greater than zero."
                );
            }

            var existing = await _productRepo.GetByIdAsync(id);

            if (existing == null || !existing.IsActive)
            {
                throw new NotFoundException("Product does not exist.");
            }

            var category = await _categoryRepo.GetByIdAsync(product.CategoryId);

            if (category == null || !category.IsActive)
            {
                throw new NotFoundException("Invalid category.");
            }

            var newName = product.ProductName.Trim();

            bool duplicateExists = await _productRepository.ExistsAsync(p =>
                p.ProductId != existing.ProductId &&
                p.ProductName.ToLower() == newName.ToLower() &&
                p.CategoryId == product.CategoryId &&
                p.IsActive);

            if (duplicateExists)
            {
                throw new AlreadyExistsException(
                    "Another product with the same name already exists in this category."
                );
            }
            string? newImageUrl = null;
            string? oldImageUrl = existing.ProductImageUrl;

            if (product.ProductImage != null)
            {
                // Save new image (validated inside ImageService)
                newImageUrl = await _imageService.SaveImageAsync(
                    product.ProductImage,
                    "products"
                );

                existing.ProductImageUrl = newImageUrl;
            }
            existing.ProductName = newName;
            existing.CategoryId = product.CategoryId;
            existing.ProductDescription = product.ProductDescription;
            existing.Price = product.Price;
            existing.UpdatedDate = DateTime.UtcNow;
            existing.UpdatedBy = _currentUser.UserId;

            try
            {
                _productRepo.Update(existing);
                await _productRepo.SaveAsync();

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

        public async Task<string> GetNextSkuAsync()
        {
            var lastSku = await _productRepository.GetLastSkuAsync();

            return SkuGenerator.GenerateNextSku(lastSku);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetByCategoryIdAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);

            if (category == null || !category.IsActive)
            {
                throw new NotFoundException("Category does not exist.");
            }

            var products = await _productRepository
                .GetByCategoryIdAsync(categoryId);

            if (!products.Any())
            {
                throw new NotFoundException("No products found in this category.");
            }

            return products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryName = category.CategoryName,
                Price = p.Price
            }).ToList();
        }

    }
}
