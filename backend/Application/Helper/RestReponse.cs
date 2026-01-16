using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public class RestReponse
    {
        public RestReponse()
        {
            Errors = new List<dynamic>();
        }
        public bool IsSuccess { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Data { get; set; }
        public List<dynamic> Errors { get; set; }

    }

    public class RestReponse<T>
        where T : class
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode Status { get; set; }
        public T Data { get; set; }
    }

    public class RestFileReponse : RestReponse
    {
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string ContentType { get; set; }
    }
}
