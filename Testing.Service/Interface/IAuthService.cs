using Pms.Dto.AuthDto;
using PmsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Interface
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<Users?> ValidateUserAsync(LoginDto dto);

    }
}
