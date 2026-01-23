using PmsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Interface
{
    public interface Ijwtservice
    {

        string GenerateToken(Users user);

        string GenerateRefreshToken();

    }
}
