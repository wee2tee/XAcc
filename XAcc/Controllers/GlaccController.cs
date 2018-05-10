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
    public class GlaccController : ControllerExtend
    {
        public GlaccController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.dbmain_context = dbmain_context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult GetAcc(int? acc_group)
        {
            this.PrepareDbContext();

            if(!acc_group.HasValue)
            {
                //return Content("Assets account");

                return PartialView("_Index", this.dbacc_context.Glacc.Where(g => g.group == "1").ToGlaccVM());
            }
            else
            {
                //return Content("Account type = " + acc_group.ToString());
                return PartialView("_Index", this.dbacc_context.Glacc.Where(g => g.group == acc_group.ToString()).ToGlaccVM());
            }
        }
    }
}