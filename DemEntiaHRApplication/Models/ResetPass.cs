using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemEntiaHRApplication.Models
{
    public class ResetPass
    {
        public int ID { get; set; }
        public string email { get; set; }
        public string token { get; set; }
    }
}
