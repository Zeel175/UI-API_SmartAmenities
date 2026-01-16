using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class UploadResidentDocumentsRequest
    {
        public List<IFormFile> Files { get; set; } = new();
    }
}
