using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.Services
{
    public class AmenityDocumentService : IAmenityDocumentService
    {
        private readonly IAmenityDocumentRepository _repository;
        private readonly IClaimAccessorService _claimAccessorService;

        public AmenityDocumentService(
            IAmenityDocumentRepository repository,
            IClaimAccessorService claimAccessorService)
        {
            _repository = repository;
            _claimAccessorService = claimAccessorService;
        }

        public async Task<List<AmenityDocument>> SaveDocumentsAsync(
            long amenityId,
            List<IFormFile> files,
            string webRootPath)
        {
            var savedDocuments = new List<AmenityDocument>();

            if (files == null || files.Count == 0)
            {
                return savedDocuments;
            }

            var root = string.IsNullOrWhiteSpace(webRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : webRootPath;

            var uploadFolder = Path.Combine(root, "uploads", "amenities", "documents");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var userId = _claimAccessorService.GetUserId();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName);
                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadFolder, storedFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new AmenityDocument
                {
                    AmenityMasterId = amenityId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/amenities/documents/{storedFileName}",
                    ContentType = file.ContentType,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now
                };

                await _repository.AddAsync(document, userId.ToString(), "Insert");
                savedDocuments.Add(document);
            }

            return savedDocuments;
        }

        public async Task<List<AmenityDocument>> GetDocumentsByAmenityAsync(long amenityId)
        {
            return await _repository
                .Get(d => d.AmenityMasterId == amenityId && d.IsActive)
                .OrderBy(d => d.FileName)
                .ToListAsync();
        }
    }
}
