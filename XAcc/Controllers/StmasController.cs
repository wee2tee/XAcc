using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XAcc.Models;

namespace XAcc.Controllers
{
    public class StmasController : Controller
    {
        private readonly DBContext db_context;

        public StmasController(DBContext db_context)
        {
            this.db_context = db_context;
        }

        public IActionResult Index()
        {
            ViewBag.stmas = this.db_context.Stmas.OrderBy(s => s.stkcod).ToList();
            
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(string stkcod, string stkdes, string stkdes2, string sellpr1)
        {
            db_context.Stmas.Add(new Stmas
            {
                stkcod = stkcod,
                stkdes = stkdes,
                stkdes2 = stkdes2,
                sellpr1 = Convert.ToDecimal(sellpr1)
            });
            db_context.SaveChanges();

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var delete_item = this.db_context.Stmas.Where(s => s.ID == Convert.ToInt32(id)).FirstOrDefault();
            if(delete_item != null)
            {
                this.db_context.Stmas.Remove(delete_item);
                this.db_context.SaveChanges();
            }

            return RedirectToAction("index");
        }
    }
}