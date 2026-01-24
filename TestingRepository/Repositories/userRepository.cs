using Microsoft.EntityFrameworkCore;
using PmsRepository.Interface;
using PmsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmsRepository.Repositories
{
    public class userRepository : IuserRepository
    {
        private readonly PMSDbContext _pmsdbcontext;

        public userRepository(PMSDbContext pMSDbContext)
        {
            _pmsdbcontext = pMSDbContext;
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await _pmsdbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Users?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _pmsdbcontext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
