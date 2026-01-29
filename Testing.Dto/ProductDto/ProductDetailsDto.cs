namespace Pms.Dto.ProductDto.ProductDto
{
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string ProductDescription { get; set; } = null!;
        public string? ProductImageUrl { get; set; }
        public string Sku { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
