using Pms.Dto.ProfileDto;
using Pms.Service.Interface;
using PmsRepository.Interface;
using PmsRepository.Models;
using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IuserRepository _userRepo;

        public ProfileService(IGenericRepository<Users> userRepository, IuserRepository userRepo)
        {
            _userRepository = userRepository;
            _userRepo = userRepo;
        }

        public async Task<UserProfileDto> GetProfileAsync(ClaimsPrincipal principal)
        {
            int userId = GetUserId(principal);

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found with id:"+userId);

            return MapToDto(user);
        }

        public async Task UpdateProfileAsync(ClaimsPrincipal principal, UpdateProfileDto dto)
        {
            int userId = GetUserId(principal);

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found with id:"+userId);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            await _userRepo.UpdateAsync(user);
        }

        private static int GetUserId(ClaimsPrincipal principal)
        {
            return int.Parse(
                principal.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
        }

        private static UserProfileDto MapToDto(Users user)
        {
            return new UserProfileDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}
