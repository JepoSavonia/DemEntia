using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DemEntiaHRApplication.Models
{
    public class UserViewModel
    {
        [DisplayName("Käyttäjänimi")]
        public string UserName { get; set; }

        [DisplayName("Salasana")]
        public string Password { get; set; }
    }
}
