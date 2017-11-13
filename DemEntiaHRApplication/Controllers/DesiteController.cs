using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemEntiaHRApplication.Models;
using Savonia.AdManagement;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
//using Savonia.AdManagement;

namespace DemEntiaHRApplication.Controllers
{
    public class DesiteController : Controller
    {
        private AccountManagementConfig AccountManagementConfig;

        public DesiteController(IOptions<AccountManagementConfig> options)
        {
            AccountManagementConfig = options?.Value;
            
        }

        public IActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="Aluenimi3.local\\UG_Admin")]
        public IActionResult Admin()
        {
            BetterAdManager adManager = new BetterAdManager(AccountManagementConfig);
            
            SavoniaUserObject userObject = adManager.FindUser(user.UserName);

            return View(user);
        }

        public IActionResult Admin()
        {
            SavoniaUserObject a = new SavoniaUserObject {
                DisplayName = "Jepo",
                Email = "asd@asd.com",
                IsEnabled = true,
                Title ="Hepo",
                Username = "jepoUser",
                Dn = "asd",
                Path = "pathasd123"};
            
            return View(a);
        }

        [Authorize(Roles = "Aluenimi3.local\\UG_Employee")]
        public IActionResult ADUser()
        {
            return View();
        }

        [Authorize(Roles = "Aluenimi3.local\\UG_HR")]
        public IActionResult HRUser()
        {
            return View();
        }
    }
}