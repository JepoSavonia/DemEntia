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
        private BetterAdManager adManager;

        public DesiteController(IOptions<AccountManagementConfig> options)
        {
            AccountManagementConfig = options?.Value;
            adManager = new BetterAdManager(AccountManagementConfig);
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
            
            SavoniaUserObject userObject = new SavoniaUserObject();

            String userName = User.Identity.Name.Replace("ALUENIMI3\\","");

            userObject = adManager.FindUser(userName);

            return View(a);
        }

        [HttpGet]
        [Authorize(Roles = "Aluenimi3.local\\UG_Employee")]
        public IActionResult ADUser()
        {
            SavoniaUserObject userObject = new SavoniaUserObject();
            String userName = User.Identity.Name.Replace("ALUENIMI3\\", "");
            userObject = adManager.FindUser(userName);
            userObject.Username = userName;
            ViewData["message"] = "Tervetuloa " + userName;
            return View(userObject);
        }

        [HttpPost]
        [Authorize(Roles = "Aluenimi3.local\\UG_Employee")]
        public IActionResult ADUser(SavoniaUserObject userObject)
        {
            userObject.Username = User.Identity.Name.Replace("ALUENIMI3\\", "");
            var booli = User.Identity.IsAuthenticated;
            var booli2 = User.Identity.IsAuthenticated;
            adManager.UpdateUser(userObject);
            ViewData["message"] = "Käyttäjän tiedot päivitetty";
            return View(userObject);
        }

        [Authorize(Roles = "Aluenimi3.local\\UG_HR")]
        public IActionResult HRUser()
        {
            SavoniaUserObject userObject = new SavoniaUserObject();
            String userName = User.Identity.Name.Replace("ALUENIMI3\\", "");
            userObject = adManager.FindUser(userName);
            return View();
        }

        
    }
}