using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XAcc.Models;

namespace XAcc.Controllers
{
    public class GljnlController : ControllerExtend
    {
        public GljnlController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.dbmain_context = dbmain_context;
        }

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