using Microsoft.EntityFrameworkCore;
using PmsRepository.Interface;
using PmsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PmsRepository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(PMSDbContext context)
        : base(context)
        {
        }
        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories.AnyAsync(predicate);
        }
    }
}
