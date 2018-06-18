using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XAcc.Controllers
{
    public class GljnlController : Controller
    {
        [Authorize]
        public IActionResult Index(string trnstat = "U")
        {
            if (trnstat == "U")
                return View("IndexUnpost");

            if (trnstat == "P")
                return View("IndexPost");

            return View("IndexUnpost");
        }
    }
}