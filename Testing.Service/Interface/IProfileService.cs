using Pms.Dto.ProfileDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Interface
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileAsync(ClaimsPrincipal user);
        Task UpdateProfileAsync(ClaimsPrincipal user, UserProfileUpdateDto dto);
    }
}
