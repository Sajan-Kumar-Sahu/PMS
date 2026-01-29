using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Dto.ProductDto
{
    public class ProductCreateDto
    {
        public string ProductName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public IFormFile? ProductImage { get; set; }


    }
}
