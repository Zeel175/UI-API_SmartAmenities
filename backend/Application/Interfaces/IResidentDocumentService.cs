using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public class DocumentLinkResult
    {
        public List<string> Linked { get; set; } = new();
        public List<string> NotFound { get; set; } = new();
        public List<string> Conflicts { get; set; } = new(); // already assigned to other resident
    }
    public interface IResidentDocumentService
    {
        Task<List<ResidentDocument>> SaveDocumentsAsync(
        long? residentId,
        List<IFormFile> files,
        long userId,
        string webRootPath
    );
        Task<DocumentLinkResult> SaveDocumentPathsAsync(long residentId, IEnumerable<string> filePaths, long userId);

        //Task SaveDocumentPathsAsync(long residentId, IEnumerable<string> filePaths, long userId);
        Task<List<ResidentDocument>> GetDocumentsByResidentAsync(long residentId);
        Task DeleteDocumentAsync(long documentId, long userId);
    }
}
