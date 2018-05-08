using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XAcc.Controllers
{
    public class GlaccController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}