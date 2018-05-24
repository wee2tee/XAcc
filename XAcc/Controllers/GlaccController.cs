using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

            ViewBag.summary_acc = this.dbacc_context.Glacc.Where(a => a.acctyp == "1").ToList();

            List<GlaccVM> acc = new List<GlaccVM>()
            {
                this.dbacc_context.Glacc.Where(g => g.group == "1" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "2" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "3" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "4" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context),
                this.dbacc_context.Glacc.Where(g => g.group == "5" && g.level == 1).FirstOrDefault()?.ToGlaccVM(this.dbacc_context)
            };
            
            if(TempData["selected_id"] != null)
            {
                ViewBag.SelectedItem = this.dbacc_context.Glacc.Where(g => g.id == (int)TempData["selected_id"]).FirstOrDefault();
            }

            return View("Index", acc.Where(a => a != null).ToList());
        }

        [HttpGet, Authorize]
        public IActionResult GetGlacc(string acctyp)
        {
            this.PrepareDbContext();

            if(acctyp != null)
            {
                return Json(this.dbacc_context.Glacc.Where(g => g.acctyp == acctyp).OrderBy(g => g.accnum).ToList());
            }
            else
            {
                return Json(this.dbacc_context.Glacc.OrderBy(g => g.accnum).ToList());
            }
        }

        [HttpGet, Authorize]
        public IActionResult GetGlaccJson(string acctyp)
        {
            this.PrepareDbContext();

            List<Glacc> accs;

            if(acctyp != null)
            {
                accs = this.dbacc_context.Glacc.Where(g => g.acctyp == acctyp).OrderBy(g => g.accnum).ToList();
            }
            else
            {
                accs = this.dbacc_context.Glacc.OrderBy(g => g.accnum).ToList();
            }

            List<GlaccJson> json_data = new List<GlaccJson>();
            foreach (Glacc acc in accs)
            {
                json_data.Add(new GlaccJson
                {
                    id = acc.id.ToString(),
                    parent = acc.parent != null && acc.parent.Trim().Length > 0 ? accs.Where(g => g.accnum.Trim() == acc.parent).FirstOrDefault().id.ToString() : "#",
                    text = acc.accnum + " " + acc.accnam,
                    state = new GlaccJsonState { disabled = false, opened = false, selected = false },
                    icon = acc.acctyp == "0" ? "jstree-file" : "",
                    //a_attr = new List<KeyValuePair<string, string>> {  new KeyValuePair<string, string>("style", "color: red")},
                    //li_attr = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("style", "font-weight: bold")}
                    //a_attr = new List<List<string>> { new List<string> { "style", "color: red" } },
                    //li_attr = new List<List<string>> { new List<string> { "aria", "font-weight: bold" } }

                });
            }

            return Json(json_data);
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
                TempData["selected_id"] = acc.id;

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
                TempData["selected_id"] = acc_to_update.id;
                
                return RedirectToAction("Index");
            }
        }

        [HttpPost, Authorize]
        public IActionResult AddEditAsync([FromBody] Glacc acc)
        {
            if (acc == null)
                return Json(new AddEditResult { result = false, message = "Null value is passed." });

            this.PrepareDbContext();

            if (acc.id == -1) // insert
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
                //TempData["selected_id"] = acc.id;

                return Json(new AddEditResult { result = true, message = acc.id.ToString() }); //RedirectToAction("Index");
            }
            else // update
            {
                var acc_to_update = this.dbacc_context.Glacc.Find(acc.id);

                if (acc_to_update == null)
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
                //TempData["selected_id"] = acc_to_update.id;

                return Json(new AddEditResult { result = true, message = acc_to_update.id.ToString() }); //RedirectToAction("Index");
            }
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