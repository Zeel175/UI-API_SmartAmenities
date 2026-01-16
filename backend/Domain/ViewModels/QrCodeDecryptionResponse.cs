using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class QrCodeDecryptionResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? QrCodeValue { get; set; }
    }
}
