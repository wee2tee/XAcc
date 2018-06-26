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
    public class IstabController : ControllerExtend
    {
        public IstabController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.dbmain_context = dbmain_context;
            this.configuration = configuration;
        }

        [Authorize, HttpGet]
        public IActionResult Index(string tabtyp = "01")
        {
            this.PrepareDbContext();

            //var istab = this.dbacc_context.
            ViewBag.tabtyp = Enum.GetValues(typeof(Istab.TABTYP)).Cast<Istab.TABTYP>().Where(i => i.GetTabtypCode() == tabtyp).FirstOrDefault();
            var model = this.dbacc_context.Istab.Where(i => i.tabtyp.Trim() == tabtyp.Trim()).OrderBy(i => i.typcod).ToList();
            return View(model);
        }

        [Authorize, HttpGet]
        public IActionResult GetIstab(int? id)
        {
            if (!id.HasValue)
                return Json("Error, id not passing");

            this.PrepareDbContext();

            var istab = this.dbacc_context.Istab.Where(i => i.id == id).FirstOrDefault();
            if(istab != null)
            {
                return Json(istab);
            }
            else
            {
                return Json("Error, Data not found");
            }
        }
    }
}