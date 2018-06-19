using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class Gljnlit
    {
        public int id { get; set; }
        public string seqit { get; set; }
        public int gljnlId { get; set; }
        public int glaccId { get; set; }
        public string depcod { get; set; }
        public string jobcod { get; set; }
        public string trntyp { get; set; }
        public Decimal amount { get; set; }


        public Gljnl gljnl { get; set; }
        public Glacc glacc { get; set; }
    }
}
