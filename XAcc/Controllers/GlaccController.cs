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
    public class GlaccController : ControllerExtend
    {
        public GlaccController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.dbmain_context = dbmain_context;
        }

        public IActionResult Index(int? id)
        {
            this.PrepareDbContext();

            ViewBag.summary_acc = this.dbacc_context.Glacc.Where(a => a.acctyp == "1").ToList();

            List<GlaccVM> acc = new List<GlaccVM>()
            {
                this.dbacc_context.Glacc.Where(g => g.group == "1" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "2" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "3" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "4" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "5" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context)
            };

            if (id.HasValue)
            {
                ViewBag.SelectedItem = this.dbacc_context.Glacc.Find(id);
            }

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

        [HttpGet, Authorize]
        public IActionResult IsDuplicateAccnum(string accnum)
        {
            this.PrepareDbContext();

            if(this.dbacc_context.Glacc.Where(g => g.accnum.Trim() == accnum.Trim()).FirstOrDefault() != null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost, Authorize]
        public IActionResult AddEdit([FromForm] Glacc acc)
        {
            if (acc == null)
                return Json("Error");

            this.PrepareDbContext();

            if(acc.id == -1) // insert
            {
                acc.id = 0;
                acc.nature = acc.group == "1" || acc.group == "5" ? "0" : "1";
                acc.status = "A";
                acc.usejob = "N";
                acc.consol = string.Empty;
                acc.creby = this.GetIdentityClaimValue(ClaimTypes.Name);
                acc.credat = DateTime.Now;
                acc.chgby = null;
                acc.chgdat = null;

                this.dbacc_context.Glacc.Add(acc);
                this.dbacc_context.SaveChanges();
                return RedirectToAction("Index");
            }
            else // update
            {
                var acc_to_update = this.dbacc_context.Glacc.Find(acc.id);

                if(acc_to_update == null)
                {
                    return Json("Not Found!");
                }
                acc_to_update.accnam = acc.accnam;
                acc_to_update.accnam2 = acc.accnam2;
                acc_to_update.acctyp = acc.acctyp;
                acc_to_update.group = acc.group;
                acc_to_update.level = acc.level;
                acc_to_update.parent = acc.parent;
                acc_to_update.usedep = acc.usedep;
                acc_to_update.nature = acc.group == "1" || acc.group == "5" ? "0" : "1";
                acc_to_update.status = "A";
                acc_to_update.usejob = "N";
                acc_to_update.consol = string.Empty;
                acc_to_update.chgby = this.GetIdentityClaimValue(ClaimTypes.Name);
                acc_to_update.chgdat = DateTime.Now;

                this.dbacc_context.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.message = new List<ViewMessage>()
            //{

            //};
        }

        [Authorize]
        public IActionResult Delete(int? form_action_id)
        {
            if (!form_action_id.HasValue)
                return Json("Null id passing");

            return Json(form_action_id);
        }
    }
}