using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class SelectListModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string UOM { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string ModelNumber { get; set; }
        public string DrawingNumber { get; set; }
        public string ProductType { get; set; }

        public string ProductLevel { get; set; }
        public string InterStateTax { get; set; }
        public string Description { get; set; }


        public List<UOMDto> UOMs { get; set; }
    }
    public class UOMDto
    {
        public int UOMId { get; set; }
        public string UOMName { get; set; }
    }
}
