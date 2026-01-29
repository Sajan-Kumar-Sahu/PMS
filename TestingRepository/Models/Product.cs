using System;
using System.Collections.Generic;

namespace PmsRepository.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public string? ProductDescription { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public string? ProductImageUrl { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual SubCategory? SubCategory { get; set; }
}
