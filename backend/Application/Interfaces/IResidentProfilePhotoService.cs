using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IResidentProfilePhotoService
    {
        Task<ResidentProfilePhoto> SaveProfilePhotoAsync(
            string fileName,
            string filePath,
            string contentType,
            long userId);
    }
}
