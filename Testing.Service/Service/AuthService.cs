using Pms.Dto.AuthDto;
using Pms.Service.Interface;
using PmsRepository.Interface;
using PmsRepository.Models;
using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IuserRepository _userRepository;

        public AuthService(IuserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            {
                if (existing != null)
                {
                    throw new AlreadyExistsException("Email already exists");
                }

                var users=new Users
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepository.AddAsync(users);
                await _userRepository.SaveAsync();
            }
        }

        public async Task<Users?> ValidateUserAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !user.IsActive)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            return isValid ? user : null;
        }
    }
}
