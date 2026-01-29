using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Dto.ProductDto
{
    public  class ProductUpdateDto
    {

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string? ProductDescription { get; set; }

        public decimal Price { get; set; }
        public IFormFile? ProductImage { get; set; }
    }
}
