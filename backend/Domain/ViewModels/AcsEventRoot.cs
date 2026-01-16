using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class AcsEventRoot
    {
        public AcsEventWrapper AcsEvent { get; set; }
    }

    public class AcsEventWrapper
    {
        public string searchID { get; set; }
        public int totalMatches { get; set; }
        public string responseStatusStrg { get; set; } // OK / MORE
        public int numOfMatches { get; set; }
        public List<AcsEventInfo> InfoList { get; set; } = new();
    }

    public class AcsEventInfo
    {
        public int major { get; set; }
        public int minor { get; set; }
        public string time { get; set; }
        public string cardNo { get; set; }
        public string name { get; set; }
        public int cardReaderNo { get; set; }
        public int doorNo { get; set; }
        public string employeeNoString { get; set; }
        public int serialNo { get; set; }
        public string userType { get; set; }
        public string currentVerifyMode { get; set; }
        public string mask { get; set; }
    }

}
