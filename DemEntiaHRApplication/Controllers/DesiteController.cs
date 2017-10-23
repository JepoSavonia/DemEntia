using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemEntiaHRApplication.Controllers
{
    public class DesiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult ADUser()
        {
            return View();
        }

        public IActionResult HRUser()
        {
            return View();
        }
    }
}