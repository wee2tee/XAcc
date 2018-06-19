using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class Gljnl
    {
        public int id { get; set; }
        public string jnltyp { get; set; }
        public string batch { get; set; }
        public DateTime voudat { get; set; }
        public string voucher { get; set; }
        public string refnum { get; set; }
        public string srcjnl { get; set; }
        public string descrp { get; set; }
        public string reverse { get; set; }
        public string trnstat { get; set; }
        public string docstat { get; set; }
        public string creby { get; set; }
        public DateTime credat { get; set; }
        public string chgby { get; set; }
        public DateTime? chgdat { get; set; }
        public string postby { get; set; }
        public DateTime? postdat { get; set; }
        public string prnby { get; set; }
        public DateTime? prndat { get; set; }
        public int prncnt { get; set; }
        public string apprby { get; set; }
        public DateTime? apprdat { get; set; }

        public ICollection<Gljnlit> gljnlit { get; set; }

    }
}
