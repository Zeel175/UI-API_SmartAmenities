using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ResidentProfilePhotoService : IResidentProfilePhotoService
    {
        private readonly IResidentProfilePhotoRepository _repository;

        public ResidentProfilePhotoService(IResidentProfilePhotoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResidentProfilePhoto> SaveProfilePhotoAsync(
            string fileName,
            string filePath,
            string contentType,
            long userId)
        {
            var photo = new ResidentProfilePhoto
            {
                FileName = fileName,
                FilePath = filePath,
                ContentType = contentType,
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                ModifiedBy = userId,
                ModifiedDate = DateTime.Now
            };

            await _repository.AddAsync(photo, userId.ToString(), "Insert");
            return photo;
        }
    }
}
