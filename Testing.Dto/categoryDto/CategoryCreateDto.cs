using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Dto.categoryDto
{
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = null!;

        public string? CategoryDescription { get; set; }

    }
}
