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

        [Authorize(Policy = "Customer")]
        public IActionResult SelectComp()
        {
            var dbsecure_context = this.GetCustomerIdentity(this.dbmain_context).EnsureDbSecureCreated(this.configuration);
            ViewBag.sccomp = dbsecure_context.Sccomp.OrderBy(s => s.compnam).ToList();


            // test get claim value
            User.Identities.First().AddClaim(new Claim(ClaimTypes.UserData, "testpump"));
            ViewBag.claim = User.Identities.First().Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

            
            return View();
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
        public async Task<IActionResult> Login(string sernum)
        {
            if (string.IsNullOrEmpty(sernum))
            {
                return RedirectToAction(nameof(Index));
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();
            if (customer != null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.SerialNumber, customer.sernum),
                    new Claim(ClaimTypes.Name, customer.prenam + " " + customer.compnam),
                    new Claim(ClaimTypes.Role, "customer")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

                customer.EnsureDbSecureCreated(this.configuration);

                return RedirectToAction("SelectComp");
            }
            else
            {
                return RedirectToAction("Index");
            }


            //return RedirectToAction("SelectComp");
        }

        [HttpPost]
        public async Task<IActionResult> AjaxLogin([FromBody] LoginInfo login_info)
        {
            if (string.IsNullOrEmpty(login_info.sernum) || 
                string.IsNullOrEmpty(login_info.phrase) || 
                string.IsNullOrEmpty(login_info.username) || 
                string.IsNullOrEmpty(login_info.pass))
            {
                return RedirectToAction(nameof(Index));
            }

            var customer = this.dbmain_context.Customer.Where(c => c.sernum.Trim() == login_info.sernum.Trim() && c.pass_phrase.Trim() == login_info.phrase.Trim()).FirstOrDefault();
            if (customer != null)
            {
                //var dbsecure_context = this.EnsureDbSecureCreated(this.dbmain_context, this.configuration);
                var dbsecure_context = customer.EnsureDbSecureCreated(this.configuration);

                var user = dbsecure_context.Scuser.Where(u => u.reccod.Trim() == login_info.username.Trim() && u.userpwd.Trim() == login_info.pass.Trim() && u.status != "X").FirstOrDefault();

                if(user != null) // Login completed
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

                    login_info.phrase = "****";
                    login_info.pass = "****";
                    login_info.login_result = true;
                    return Json(login_info);
                }
                else // User/Password is incorrect
                {
                    return Json("User name/Password is incorrect.");
                }
                
            }
            else
            {
                return Json("Serial number/Phrase is incorrect.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //public IActionResult AjaxTest([FromBody] LoginInfo login_info/*string sernum, string phrase, string username, string pass*/)
        //{
        //    //return new JsonResult(login_info.username);
        //    return Json(login_info);
        //}

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
}
