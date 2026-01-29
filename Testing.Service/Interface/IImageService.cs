using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service.Interface
{
    public interface IImageService
    {
        Task<string?> SaveImageAsync(IFormFile file, string folderName);
        void DeleteImage(string imagePath);
    }
}
