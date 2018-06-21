using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XAcc.Models;

namespace XAcc.Controllers
{
    public class IstabController : ControllerExtend
    {
        public IstabController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.dbmain_context = dbmain_context;
            this.configuration = configuration;
        }

        public IActionResult Index(string tabtyp = "01")
        {
            this.PrepareDbContext();

            //var istab = this.dbacc_context.
            ViewBag.tabtyp = Enum.GetValues(typeof(Istab.TABTYP)).Cast<Istab.TABTYP>().Where(i => i.GetTabtypCode() == tabtyp).FirstOrDefault();
            return View();
        }
    }
}