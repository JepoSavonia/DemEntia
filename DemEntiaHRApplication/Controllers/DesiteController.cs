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
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Net.Mail;
using DemEntiaHRApplication.Data;
using System.Linq;

//using Savonia.AdManagement;

namespace DemEntiaHRApplication.Controllers
{
    public class DesiteController : Controller
    {

        private readonly DementiaContext _context;
        private AccountManagementConfig AccountManagementConfig;
        private BetterAdManager adManager;


        public DesiteController(IOptions<AccountManagementConfig> options, DementiaContext context)
        {
            AccountManagementConfig = options?.Value;
            adManager = new BetterAdManager(AccountManagementConfig);
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string email)
        {
            SavoniaUserObject obj = new SavoniaUserObject();

            obj = adManager.FindUserByEmail(email);
            if (obj != null)
            {
                ViewBag.username = obj.Username;
                ViewBag.email = obj.Email;

                string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                token = token.Replace("+", "");
                token = token.Replace("/", "");
                token = token.Replace("=", "");
                _context.ResetPass.Add(new ResetPass { token = token, email = obj.Email });
                _context.SaveChanges();

                string resetUlr = "http://localhost:50191/Desite/ResetPassword?token="+token;


                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(obj.Email));
                message.Subject = "Testiä";
                message.From = new MailAddress("mailsender@ALUENIMI3.LOCAL");
                message.Subject = "Your email subject";
                message.Body = string.Format(body, "Dementia", "mailsender@aluenimi3.local", "Tällä linkillä pääset vaihtamaan salasanasi: <br/><a href='"+ resetUlr +"'>" + resetUlr + "</a>");
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credentials = new NetworkCredential
                    {
                        UserName = "mailsender@ALUENIMI3.LOCAL",
                        Password = "Salasana123"
                    };

                    smtp.Credentials = credentials;
                    smtp.Host = "de-exch1.ALUENIMI3.LOCAL";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    
                }

                
            }

            ViewBag.error = "Sähköposti lähetetty osoitteeseen " + email;
            

            return RedirectToAction("Index");
        }

        public IActionResult ResetPassword(string token)
        {
            
            ResetPass userToken = _context.ResetPass.SingleOrDefault(m => m.token == token);
            return View(userToken);
        }

        [HttpPost]
        public IActionResult SetNewPass(string password1, string password2, string email, string token)
        {
            if (password1 == password2)
            {
                SavoniaUserObject obj = new SavoniaUserObject();

                obj = adManager.FindUserByEmail(email);
                if (obj != null)
                {
                    adManager.ResetPassword(obj.Username, password1);

                    ViewBag.error = "Salasana vaihdettu";
                    try
                    {
                        ResetPass userToken = _context.ResetPass.SingleOrDefault(m => m.token == token);
                        _context.Remove(userToken);
                        _context.SaveChanges();
                    }
                    catch(Exception ex)
                    {  }
                }
            }
            else
            {
                RedirectToAction("ResetPassword", new { token = token });
            }
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Admin(SavoniaUserObject userObject, string update, string createUser)
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

                await TukiEmail(userObject);

                return View();
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

        public async Task<IActionResult> SaveUsers(string userArray)
        {
           
            List<SavoniaUserObject> userList = JsonConvert.DeserializeObject<List<SavoniaUserObject>>(userArray);

            foreach (var user in userList)
            {
                adManager.AddUser(user);
                adManager.AddUserToGroup(user.Username, "UG_Employee");

                await TukiEmail(user);

            }

            return Json(new { success = true, responseText = "Käyttäjät lisätty!" });
        }

        public async Task TukiEmail(SavoniaUserObject userObject)
        {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress("ittuki@aluenimi3.local"));
            message.Subject = "Uusi käyttäjä";
            message.From = new MailAddress("mailsender@ALUENIMI3.LOCAL");
            //message.Subject = "Your email subject";
            message.Body = string.Format(body, "Dementia", "mailsender@aluenimi3.local", "Uusikäyttäjä: " + userObject.Username);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credentials = new NetworkCredential
                {
                    UserName = "mailsender@ALUENIMI3.LOCAL",
                    Password = "Salasana123"
                };

                smtp.Credentials = credentials;
                smtp.Host = "de-exch1.ALUENIMI3.LOCAL";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);

            }
        }

    }
}