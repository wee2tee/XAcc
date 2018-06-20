using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class Istab
    {
        public enum TABTYP
        {
            BNKCOD,
            CHQSTAT,
            QUCOD,
            BRANCH,
            STKGRP,
            STKLEVEL,
            AREA,
            DLVBY,
            CANCEL_SO_REASON,
            CANCEL_PO_REASON,
            QT_STATUS,
            CUSTYP,
            SUPTYP,
            SLMTYP,
            DEPARTMENT,
            FASGRP,
            PCASH_STATUS,
            TAXGRP
        }

        public int id { get; set; }
        public string tabtyp { get; set; }
        public string typcod { get; set; }
        public string shortnam { get; set; }
        public string shortnam2 { get; set; }
        public string typdes { get; set; }
        public string typdes2 { get; set; }
        public string fld01 { get; set; }
        public decimal fld02 { get; set; }
        public string depcod { get; set; }
        public string status { get; set; }
        public string creby { get; set; }
        public DateTime? credat { get; set; }
        public string chgby { get; set; }
        public DateTime? chgdat { get; set; }
    }


}
