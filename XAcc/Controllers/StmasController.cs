using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XAcc.Models;
using XAcc.Models.Secure;
using System.Security.Claims;

namespace XAcc.Controllers
{
    public class StmasController : ControllerExtend
    {
        //private readonly DBContext db_context;
        //private readonly DBContext2 db_context2;
        //private readonly IConfiguration configuration;
        //private readonly DBMainContext dbmain_context;
        //private DBSecureContext dbsecure_context;
        //private DBAccContext dbacc_context;
        private Sccomp sccomp;

        public StmasController(DBMainContext dbmain_context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.dbmain_context = dbmain_context;
        }

        [Authorize(Policy = "Customer")]
        public IActionResult Index()
        {
            this.PrepareDbContext();
            ViewBag.stmas = this.dbacc_context.Stmas.OrderBy(s => s.stkcod).ToList();
            
            return View();
        }

        [HttpPost, Authorize(Policy = "Customer")]
        public IActionResult Add(string stkcod, string stkdes, string stkdes2, string sellpr1)
        {
            this.PrepareDbContext();

            this.dbacc_context.Stmas.Add(new Stmas
            {
                stkcod = stkcod,
                stkdes = stkdes,
                stkdes2 = stkdes2,
                sellpr1 = Convert.ToDecimal(sellpr1)
            });
            this.dbacc_context.SaveChanges();

            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            this.PrepareDbContext();

            var delete_item = this.dbacc_context.Stmas.Where(s => s.ID == Convert.ToInt32(id)).FirstOrDefault();
            if (delete_item != null)
            {
                this.dbacc_context.Stmas.Remove(delete_item);
                this.dbacc_context.SaveChanges();
            }

            return RedirectToAction("index");
        }
    }
}