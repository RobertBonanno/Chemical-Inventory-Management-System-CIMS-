using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Models.OrderViewModels
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime Orderdate { get; set; }
        public DateTime Recievedate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int VendorID { get; set; }
    }
}
