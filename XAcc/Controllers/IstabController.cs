using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        [Authorize, HttpGet]
        public IActionResult CheckDuplicate(string tabtyp, string typcod)
        {
            this.PrepareDbContext();

            typcod = typcod != null ? typcod : string.Empty;

            if(this.dbacc_context.Istab.Where(i => i.tabtyp == tabtyp && i.typcod == typcod).AsEnumerable().Count() == 0)
            {
                return Json(new { result = true, message = string.Empty });
            }
            else
            {
                return Json(new { result = false, message = "รหัส \'" + typcod + "\' นี้มีอยู่แล้ว" });
            }
        }

        [Authorize, HttpPost]
        public IActionResult AddEditAsync([FromBody] Istab istab)
        {
            if (istab == null)
                return Json(new AddEditResult { result = false, message = "Incorrect data posted." });

            this.PrepareDbContext();

            istab.fld01 = istab.fld01 != null ? istab.fld01 : string.Empty;
            istab.status = istab.status != null ? istab.status : string.Empty;
            istab.depcod = istab.depcod != null ? istab.depcod : string.Empty;

            if (istab.id == -1) // Add
            {
                if (this.dbacc_context.Istab.Where(i => i.tabtyp == istab.tabtyp && i.typcod == istab.typcod).AsEnumerable().Count() > 0)
                {
                    return Json(new AddEditResult { result = false, message = "รหัส \'" + istab.typcod + "\' นี้มีอยู่แล้ว" });
                }

                istab.id = 0;
                istab.creby = this.GetIdentityClaimValue(ClaimTypes.Name);
                istab.credat = DateTime.Now;

                this.dbacc_context.Istab.Add(istab);
                this.dbacc_context.SaveChanges();

                return Json(new AddEditResult { result = true, message = istab.id.ToString() });
            }
            else // Edit
            {
                var istab_to_update = this.dbacc_context.Istab.Find(istab.id);

                if(istab_to_update != null)
                {
                    istab_to_update.chgby = this.GetIdentityClaimValue(ClaimTypes.Name);
                    istab_to_update.chgdat = DateTime.Now;
                    istab_to_update.depcod = istab.depcod;
                    istab_to_update.fld01 = istab.fld01;
                    istab_to_update.fld02 = istab.fld02;
                    istab_to_update.shortnam = istab.shortnam;
                    istab_to_update.shortnam2 = istab.shortnam2;
                    istab_to_update.status = istab.status;
                    istab_to_update.typcod = istab.typcod;
                    istab_to_update.typdes = istab.typdes;
                    istab_to_update.typdes2 = istab.typdes2;

                    this.dbacc_context.SaveChanges();
                    return Json(new AddEditResult { result = true, message = istab.id.ToString() });
                }
                else
                {
                    return Json(new AddEditResult { result = false, message = "Cannot find data to update!" });
                }
            }
        }

        [Authorize, HttpPost]
        public IActionResult DeleteAsync([FromBody] Istab istab)
        {
            if (istab == null)
                return Json(new AddEditResult { result = false, message = "Incorrect data posted." });

            this.PrepareDbContext();

            var istab_to_delete = this.dbacc_context.Istab.Find(istab.id);

            if(istab_to_delete != null)
            {
                this.dbacc_context.Istab.Remove(istab_to_delete);
                this.dbacc_context.SaveChanges();
                return Json(new AddEditResult { result = true, message = "success" });
            }
            else
            {
                return Json(new AddEditResult { result = false, message = "Cannot find data to delete!" });
            }
        }
    }
}