using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pms.Dto.ProfileDto;
using Pms.Service.Interface;
using PmsRepository.Models;

namespace Pms.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _profileService.GetProfileAsync(User);
            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            await _profileService.UpdateProfileAsync(User, dto);
            return Ok("Profile Updated Successfully");
        }
    }
}
