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
            this.PrepareDbContext();

            //if(!acc_group.HasValue)
            //{
            //    var models = this.dbacc_context.Glacc.Where(g => g.group == "1" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context);
            //    return PartialView("_Index", models);
            //}
            //else
            //{
            //    var models = this.dbacc_context.Glacc.Where(g => g.group == acc_group.ToString() && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context);
            //    return PartialView("_Index", models);
            //}

            List<GlaccVM> acc = new List<GlaccVM>()
            {
                this.dbacc_context.Glacc.Where(g => g.group == "1" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "2" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "3" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "4" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "5" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context)
            };

            return View("Index", acc.Where(a => a != null).ToList());
        }

        [HttpGet, Authorize]
        public IActionResult GetAcc(int? id)
        {
            this.PrepareDbContext();
            if (id.HasValue)
            {
                return Json(this.dbacc_context.Glacc.Find(id));
            }
            else
            {
                return Json(new Glacc());
            }
        }
        //public IActionResult GetAcc(int? acc_group)
        //{
        //    this.PrepareDbContext();

        //    List<GlaccVM> acc = new List<GlaccVM>()
        //    {
        //        this.dbacc_context.Glacc.Where(g => g.group == "1" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
        //        this.dbacc_context.Glacc.Where(g => g.group == "2" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
        //        this.dbacc_context.Glacc.Where(g => g.group == "3" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
        //        this.dbacc_context.Glacc.Where(g => g.group == "4" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
        //        this.dbacc_context.Glacc.Where(g => g.group == "5" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context)
        //    };

        //    return PartialView("_Index", acc.Where(a => a != null).ToList());
        //}

        [Authorize]
        public IActionResult Delete(int? form_action_id)
        {
            if (!form_action_id.HasValue)
                return Json("Null id passing");

            return Json(form_action_id);
        }
    }
}