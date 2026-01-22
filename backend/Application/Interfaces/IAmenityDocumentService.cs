using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAmenityDocumentService
    {
        Task<List<AmenityDocument>> SaveDocumentsAsync(
            long amenityId,
            List<IFormFile> files,
            string webRootPath);

        Task<List<AmenityDocument>> GetDocumentsByAmenityAsync(long amenityId);
        Task DeleteDocumentAsync(long documentId, long userId);
    }
}
