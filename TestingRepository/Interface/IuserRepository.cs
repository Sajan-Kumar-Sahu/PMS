using PmsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmsRepository.Interface
{
    public interface IuserRepository
    {

        Task<Users?> GetByEmailAsync(string email);
        Task AddAsync(Users user);
        Task SaveAsync();
        Task UpdateAsync(Users user);

        Task<Users?> GetByRefreshTokenAsync(string refreshToken);
    }
}
