using PmsRepository.Models;
using System.Linq.Expressions;


namespace PmsRepository.Interface
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate);
    }
}
