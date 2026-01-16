using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;



namespace Application.Services
{
    public class ResidentDocumentService : IResidentDocumentService
    {
        private readonly IResidentDocumentRepository _repository;
        private readonly AppDbContext _context;
        public ResidentDocumentService
        (
            IResidentDocumentRepository repository,
            AppDbContext context
        )
        {
            _repository = repository;
            _context = context;

        }
        public async Task<List<ResidentDocument>> SaveDocumentsAsync(
           long? residentId,
           List<IFormFile> files,
           long userId,
           string webRootPath)
        {
            var savedDocuments = new List<ResidentDocument>();

            if (files == null || !files.Any())
                return savedDocuments;

            var root = string.IsNullOrWhiteSpace(webRootPath)
    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    : webRootPath;

            var uploadFolder = Path.Combine(
                root,
                "uploads",
                "residents",
                "documents"
            );


            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                var extension = Path.GetExtension(file.FileName);
                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadFolder, storedFileName);

                // 💾 Save physical file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 🧾 Save DB record
                var document = new ResidentDocument
                {
                    ResidentMasterId = residentId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/residents/documents/{storedFileName}",
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
        public async Task<DocumentLinkResult> SaveDocumentPathsAsync(long residentId, IEnumerable<string> filePaths, long userId)
        {
            var result = new DocumentLinkResult();

            if (filePaths == null) return result;

            // normalize
            var normalizedPaths = filePaths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!normalizedPaths.Any()) return result;

            var now = DateTime.Now;

            // fetch all matching docs once
            var docs = await _context.ResidentDocuments
                .Where(d => d.IsActive && normalizedPaths.Contains(d.FilePath))
                .ToListAsync();

            foreach (var path in normalizedPaths)
            {
                // pick the best candidate to update:
                // priority 1: ResidentMasterId is NULL (pre-uploaded document)
                // priority 2: already linked to same resident
                var candidates = docs
                    .Where(d => string.Equals(d.FilePath, path, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(d => d.ResidentMasterId == null) // null first
                    .ThenByDescending(d => d.Id)
                    .ToList();

                var doc = candidates.FirstOrDefault();

                if (doc == null)
                {
                    // ❌ do NOT insert new row (your requirement)
                    result.NotFound.Add(path);
                    continue;
                }

                // If already linked to another resident, do not move it (safe behavior)
                if (doc.ResidentMasterId != null && doc.ResidentMasterId != residentId)
                {
                    result.Conflicts.Add(path);
                    continue;
                }

                // ✅ update only (no new row)
                doc.ResidentMasterId = residentId;
                doc.ModifiedBy = userId;
                doc.ModifiedDate = now;

                result.Linked.Add(path);
            }

            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<List<ResidentDocument>> GetDocumentsByResidentAsync(long residentId)
        {
            return await _repository
                .Get(d => d.ResidentMasterId == residentId && d.IsActive)
                .OrderBy(d => d.FileName)
                .ToListAsync();
        }

        public async Task DeleteDocumentAsync(long documentId, long userId)
        {
            var doc = await _repository.GetByIdAsync(documentId);
            if (doc == null) return;

            doc.IsActive = false;
            doc.ModifiedBy = userId;
            doc.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(doc, userId.ToString(), "Delete");
        }

    }
}