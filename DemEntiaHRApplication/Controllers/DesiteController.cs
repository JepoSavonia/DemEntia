using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemEntiaHRApplication.Models;
using Savonia.AdManagement;

namespace DemEntiaHRApplication.Controllers
{
    public class DesiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UserViewModel user)
        {
            
            BetterAdManager adManager = new BetterAdManager(new AccountManagementConfig { Domain = "ALUENIMI3.LOCAL", Container = "OU=Users,OU=DE,DC=ALUENIMI3,DC=LOCAL", Username = user.UserName, Password = user.Password });

            SavoniaUserObject userObject = adManager.FindUser(user.UserName);

            return View(user);
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