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
        public IActionResult Admin(string username)
        {
            String adminUser = User.Identity.Name.Replace("ALUENIMI3\\", "");
            ViewData["message"] = "Tervetuloa " + adminUser;
            if (username != null)
            {
                SavoniaUserObject userObject = new SavoniaUserObject();
                userObject = adManager.FindUser(username);
                ViewData["display"] = "block";
                return View(userObject);
            }
            ViewData["display"] = "none";
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Aluenimi3.local\\UG_Admin")]
        public IActionResult Admin(SavoniaUserObject userObject, string update, string createUser)
        {
            ViewData["display"] = "block";
            if (update != null)
            {
                adManager.UpdateUser(userObject);
                ViewData["updateMessage"] = "käyttäjä " + userObject.Username + " päivitetty!";
            }
            else if (createUser != null)
            {
                adManager.AddUser(userObject);
                adManager.AddUserToGroup(userObject.Username, "UG_Employee");

                ViewData["message"] = "käyttäjä " + userObject.Username + " lisätty!";

                return View();
            }
            
            return View(userObject);
        }

        public IActionResult AddUser()
        {
            return ViewComponent("NewUser");
        }

        public IActionResult ResetPass(SavoniaUserObject userObject)
        {
            adManager.ResetPassword(userObject.Username, userObject.Password);
            ViewData["resetPassMessage"] = "Käyttäjän " + userObject.Username + " salasana vaihdettu!";
            userObject.Password = "";
            return View("Admin", userObject);
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
            adManager.UpdateUser(userObject);
            ViewData["message"] = "Käyttäjän " + userObject.Username + " tiedot päivitetty";
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