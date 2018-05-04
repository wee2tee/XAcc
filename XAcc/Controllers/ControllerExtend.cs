using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XAcc.Models;
using XAcc.Models.Secure;
using XAcc.Models.MainDB;
using Microsoft.Extensions.Configuration;

namespace XAcc.Controllers
{
    public class ControllerExtend : Controller
    {
        public DBMainContext dbmain_context { get; set; }
        public DBAccContext dbacc_context { get; set; }
        public DBSecureContext dbsecure_context { get; set; }
        public IConfiguration configuration { get; set; }
    }
}
