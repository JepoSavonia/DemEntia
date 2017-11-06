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

        /*[HttpPost]
        public IActionResult Index()
        {
            BetterAdManager adManager = new BetterAdManager(AccountManagementConfig);
            
            SavoniaUserObject userObject = adManager.FindUser(user.UserName);

            return View();
        }*/

        [Authorize(Roles ="Aluenimi3.local\\UG_Admin")]
        public IActionResult Admin()
        {
            return View();
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