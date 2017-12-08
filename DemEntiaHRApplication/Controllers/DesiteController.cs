using System;
using Microsoft.AspNetCore.Mvc;
using Savonia.AdManagement;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;
using System.IO;
using Microsoft.AspNetCore.Http;
using DemEntiaHRApplication.Models;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

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
                if (userObject != null)
                {
                    ViewData["display"] = "block";
                }
                else
                {
                    ViewData["userNotFound"] = "Käyttäjää ei löytynyt!";
                    ViewData["display"] = "none";
                }
                return View(new UserModel { User = userObject, AdminUser = adManager.FindUser(adminUser) });
            }
            ViewData["display"] = "none";
            var admin = adManager.FindUser(adminUser);
            return View(new UserModel { AdminUser = admin });
        }

        [HttpPost]
        [Authorize(Roles = "Aluenimi3.local\\UG_Admin")]
        public IActionResult Admin(SavoniaUserObject userObject, string update, string createUser)
        {
            ViewData["display"] = "block";
            String adminUser = User.Identity.Name.Replace("ALUENIMI3\\", "");
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

                
                return View(new UserModel {AdminUser= adManager.FindUser(adminUser)});
            }
            
            return View(new UserModel { User = userObject, AdminUser = adManager.FindUser(adminUser) });
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
            String adminUser = User.Identity.Name.Replace("ALUENIMI3\\", "");
            return View("Admin", new UserModel { User = userObject, AdminUser = adManager.FindUser(adminUser) });
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

        [HttpPost]
        public IActionResult PostFile(IFormFile postedFile)
        {

            List<SavoniaUserObject> results = new List<SavoniaUserObject>();

            var reader = new StreamReader(postedFile.OpenReadStream());

            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                results.Add(new SavoniaUserObject {Name = values[0], Surname=values[1], Username=values[2], Email=values[3], Password=values[4], IsEnabled=Boolean.Parse(values[5]) });
            }

            return View("HRUser", new UserModel { UserList = results });
        }

        [Authorize(Roles = "Aluenimi3.local\\UG_HR")]
        public IActionResult HRUser()
        {
            SavoniaUserObject userObject = new SavoniaUserObject();
            String userName = User.Identity.Name.Replace("ALUENIMI3\\", "");
            userObject = adManager.FindUser(userName);

            return View();
        }

        public IActionResult SaveUsers(string userArray)
        {
           
            List<SavoniaUserObject> userList = JsonConvert.DeserializeObject<List<SavoniaUserObject>>(userArray);

            foreach (var user in userList)
            {
                adManager.AddUser(user);
                adManager.AddUserToGroup(user.Username, "UG_Employee");
            }

            return Json(new { success = true, responseText = "Käyttäjät lisätty!" });
        }

    }
}