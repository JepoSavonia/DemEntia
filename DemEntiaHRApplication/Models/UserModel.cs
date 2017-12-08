using Savonia.AdManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemEntiaHRApplication.Models
{
    public class UserModel
    {
        public IEnumerable<SavoniaUserObject> UserList { get; set; }
        public SavoniaUserObject User { get; set; }
        public SavoniaUserObject AdminUser { get; set; }
    }
}
