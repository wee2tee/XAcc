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

        [Authorize]
        private void EnsureDbReady()
        {
            /* Ensure Secure DB / Dat,Test DB is created */
            if (User.Identity.IsAuthenticated)
            {
                this.dbsecure_context = this.EnsureDbSecureCreated(this.dbmain_context, this.configuration);
            }
        }

        [Authorize(Policy = "Customer")]
        public IActionResult SelectComp()
        {
            this.EnsureDbReady();
            
            //User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.UserData, "testpump") }));
            User.Identities.First().AddClaim(new Claim(ClaimTypes.UserData, "testpump"));

            ViewBag.claim = User.Identities.First().Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

            ViewBag.sccomp = this.dbsecure_context.Sccomp.OrderBy(s => s.compnam).ToList();
            return View();
        }

        [Authorize(Policy = "Customer")]
        public IActionResult Index()
        {
            this.EnsureDbReady();

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
            }
            else
            {
                ViewBag.ErrorMsg = "S/N incorrect";
            }

            //this.EnsureDbSecureCreated(this.dbmain_context, this.configuration);

            return RedirectToAction("SelectComp");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
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
}
