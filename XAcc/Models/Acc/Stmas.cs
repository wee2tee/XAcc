using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XAcc.Models
{
    public class Stmas
    {
        public int id { get; set; }
        public string stkcod { get; set; }
        public string stkdes { get; set; }
        public string stkdes2 { get; set; }
        public decimal sellpr1 { get; set; }
    }
}
