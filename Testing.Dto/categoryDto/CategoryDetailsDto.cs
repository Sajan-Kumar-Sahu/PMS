using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Dto.categoryDto
{
    public class CategoryDetailsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? CategoryDescription { get; set; }
        public string? CategoryImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
