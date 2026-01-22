using PmsRepository.Models;
using System.Linq.Expressions;


namespace PmsRepository.Interface
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    }
}
