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
        private readonly IImageService _imageService;

        public ProfileService(IGenericRepository<Users> userRepository, IImageService imageService) 
        { 
            _userRepository = userRepository;
            _imageService = imageService;
        }

        public async Task<UserProfileDto> GetProfileAsync(ClaimsPrincipal principal)
        {
            int userId = GetUserId(principal);

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found with id:"+userId);

            return MapToDto(user);
        }

        public async Task UpdateProfileAsync(ClaimsPrincipal principal, UserProfileUpdateDto dto)
        {
            int userId = GetUserId(principal);

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found with id:"+userId);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            string? newImageUrl = null;
            string? oldImageUrl = user.ProfileImageUrl;

            if (dto.ProfileImage != null)
            {
                newImageUrl = await _imageService.SaveImageAsync(
                   dto.ProfileImage,
                   "profiles"
               );
                user.ProfileImageUrl = newImageUrl;
            }
            try
            {
                _userRepository.Update(user);
                await _userRepository.SaveAsync();

                if (!string.IsNullOrEmpty(newImageUrl) &&
                    !string.IsNullOrEmpty(oldImageUrl))
                {
                    _imageService.DeleteImage(oldImageUrl);
                }

            }
            catch
            {
                if (!string.IsNullOrEmpty(newImageUrl))
                    _imageService.DeleteImage(newImageUrl);

                throw;
            }
            
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
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }
    }
}
