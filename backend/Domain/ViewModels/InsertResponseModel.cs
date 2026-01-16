using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class InsertResponseModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? QrCodeValue { get; set; }
        public string? QrCodeImagePath { get; set; }
    }
}
