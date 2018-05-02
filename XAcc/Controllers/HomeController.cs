using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using XAcc.Models;
using XAcc.Models.MainDB;
using XAcc.Models.Secure;

namespace XAcc.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBMainContext dbmain_context;
        private readonly IConfiguration configuration;
        private string acc_conn_str = string.Empty;
        private DBSecureContext dbsecure_context;

        public HomeController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.dbmain_context = dbmain_context;
            this.configuration = configuration;
        }

        //[Authorize]
        //private void EnsureDbReady()
        //{
        //    /* Ensure Secure DB / Dat,Test DB is created */
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var identity = this.User.Identity as ClaimsIdentity;
        //        var sernum = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;

        //        Customer customer = dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();

        //        this.dbsecure_context = this.EnsureDbSecureCreated(this.dbmain_context, this.configuration);
        //    }
        //}

        [HttpGet, Authorize(Policy = "Customer")]
        public IActionResult SelectComp(int? ID)
        {
            var dbsecure_context = this.GetCustomerIdentity(this.dbmain_context).EnsureDbSecureCreated(this.configuration);

            if (/*!string.IsNullOrEmpty(ID)*/ ID.HasValue)
            {
                Sccomp comp = dbsecure_context.Sccomp.Where(c => c.ID == ID).FirstOrDefault();

                if(comp != null)
                {
                    User.Identities.First().AddClaim(new Claim(ClaimTypes.Role, comp.dbname));
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                ViewBag.sccomp = dbsecure_context.Sccomp.OrderBy(s => s.compnam).ToList();
                return View();
            }


            // test get claim value
            //User.Identities.First().AddClaim(new Claim(ClaimTypes.UserData, "testpump"));
            //ViewBag.claim = User.Identities.First().Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();
        }

        [Authorize(Policy = "Customer")]
        public IActionResult Index()
        {
            //this.EnsureDbReady();
            this.GetCustomerIdentity(this.dbmain_context).EnsureDbSecureCreated(this.configuration);

            return View();
        }

        public IActionResult LoginForm()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInfo login_info)
        {
            if (login_info == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == login_info.sernum.Trim() && c.pass_phrase.Trim() == login_info.phrase.Trim()).FirstOrDefault();
            if (customer != null)
            {
                var dbsecure_context = customer.EnsureDbSecureCreated(this.configuration);
                var user = dbsecure_context.Scuser.Where(u => u.reccod.Trim() == login_info.username.Trim() && u.userpwd.Trim() == login_info.pass.Trim() && u.status != "X").FirstOrDefault();

                if (user != null) // Login completed
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.SerialNumber, customer.sernum),
                        new Claim(ClaimTypes.Name, user.reccod),
                        new Claim(ClaimTypes.Role, "customer"),
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                    return RedirectToAction("SelectComp");
                }
                else
                {
                    ViewBag.errmsg = "รหัสผู้ใช้/รหัสผ่าน ไม่ถูกต้อง";
                    return RedirectToAction("LoginForm");
                }
            }
            else
            {
                ViewBag.errmsg = "Serial number/Pass phrase ไม่ถูกต้อง";
                return RedirectToAction("LoginForm");
            }
        }

        [HttpPost]
        public IActionResult ValidateUser([FromBody] LoginInfo login_info)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(new LoginResult { result = true, return_obj = "Already login" });
            }

            if (string.IsNullOrEmpty(login_info.sernum) ||
                string.IsNullOrEmpty(login_info.phrase))
            {
                return Json(new LoginResult { result = false, return_obj = "Serial number/Phrase is incorrect." });
            }
            if(string.IsNullOrEmpty(login_info.username) || 
                string.IsNullOrEmpty(login_info.pass))
            {
                return Json(new LoginResult { result = false, return_obj = "User name/Password is incorrect." });
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == login_info.sernum.Trim() && c.pass_phrase.Trim() == login_info.phrase.Trim()).FirstOrDefault();
            if (customer != null)
            {
                var dbsecure_context = customer.EnsureDbSecureCreated(this.configuration);

                var user = dbsecure_context.Scuser.Where(u => u.reccod.Trim() == login_info.username.Trim() && u.userpwd.Trim() == login_info.pass.Trim() && u.status != "X").FirstOrDefault();

                if(user != null) // Login completed
                {
                    return Json(new LoginResult { result = true, return_obj = "Success" /*comp*/ });
                }
                else // User/Password is incorrect
                {
                    return Json(new LoginResult { result = false, return_obj = "User name/Password is incorrect." });
                }
                
            }
            else
            {
                return Json(new LoginResult { result = false, return_obj = "Serial number/Phrase is incorrect." });
            }
        }

        [HttpPost]
        public IActionResult GetDataList([FromBody] LoginInfo login_info)
        {
            if (string.IsNullOrEmpty(login_info.sernum) ||
                string.IsNullOrEmpty(login_info.phrase) ||
                string.IsNullOrEmpty(login_info.username) ||
                string.IsNullOrEmpty(login_info.pass))
            {
                return PartialView("_SelectComp");
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == login_info.sernum.Trim() && c.pass_phrase.Trim() == login_info.phrase.Trim()).FirstOrDefault();
            if (customer != null)
            {
                var dbsecure_context = customer.EnsureDbSecureCreated(this.configuration);

                var user = dbsecure_context.Scuser.Where(u => u.reccod.Trim() == login_info.username.Trim() && u.userpwd.Trim() == login_info.pass.Trim() && u.status != "X").FirstOrDefault();

                if (user != null) // Login completed
                {
                    ViewBag.login_info = login_info;
                    var comp = dbsecure_context.Sccomp.OrderBy(c => c.compnam).ToList();
                    return PartialView("_SelectComp", comp);
                }
                else // User/Password is incorrect
                {
                    return PartialView("_SelectComp");
                }

            }
            else
            {
                return PartialView("_SelectComp");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginInfo login_info)
        {
            if (string.IsNullOrEmpty(login_info.sernum) ||
                string.IsNullOrEmpty(login_info.phrase) ||
                string.IsNullOrEmpty(login_info.username) ||
                string.IsNullOrEmpty(login_info.pass) ||
                string.IsNullOrEmpty(login_info.dbname))
            {
                return RedirectToAction("LoginForm"); //Json(new LoginResult { result = false, return_obj = "Sign in failed." });
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == login_info.sernum.Trim() && c.pass_phrase.Trim() == login_info.phrase.Trim()).FirstOrDefault();
            if (customer != null)
            {
                var dbsecure_context = customer.EnsureDbSecureCreated(this.configuration);

                var user = dbsecure_context.Scuser.Where(u => u.reccod.Trim() == login_info.username.Trim() && u.userpwd.Trim() == login_info.pass.Trim() && u.status != "X").FirstOrDefault();

                if (user != null) // Login completed
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.SerialNumber, customer.sernum),
                        new Claim(ClaimTypes.Name, user.reccod),
                        new Claim(ClaimTypes.Role, "customer"),
                        new Claim(ClaimTypes.UserData, login_info.dbname)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                    return RedirectToAction("Index"); //Json(new LoginResult { result = true, return_obj = "Success" });
                }
                else // User/Password is incorrect
                {
                    return RedirectToAction("LoginForm"); //Json(new LoginResult { result = false, return_obj = "Sign in failed." });
                }

            }
            else
            {
                return RedirectToAction("LoginForm"); //Json(new LoginResult { result = false, return_obj = "Sign in failed." });
            }
        }

        //[HttpPost] // problem with multiple logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginForm");
        }

        [Authorize(Policy = "User")]
        public IActionResult ManageProfile() => View();

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ErrorNotLoggedIn() => View();

        public IActionResult ErrorForbidden() => View();
    }

    public class LoginResult
    {
        public bool result { get; set; }
        public object return_obj { get; set; }
    }
}
