using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ResponseModel
    {
        public long Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }
        public string FilePath1 { get; set; }
        public int Flag { get; set; }
        public int Count { get; set; }
        public Byte[] File { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }


    }
}
