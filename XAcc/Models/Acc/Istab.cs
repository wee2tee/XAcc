using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class Istab
    {
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

    public class Tabtyp
    {
        public const string BNKCOD = "01";
        public const string CHQSTAT = "02";
        public const string QUCOD = "20";
        public const string BRANCH = "21";
        public const string STKGRP = "22";
        public const string STKLEVEL = "23";
        public const string AREA = "40";
        public const string DLVBY = "41";
        public const string CANCEL_SO_REASON = "42";
        public const string CANCEL_PO_REASON = "43";
        public const string QT_STATUS = "44";
        public const string CUSTYP = "45";
        public const string SUPTYP = "46";
        public const string SLMTYP = "47";
        public const string DEPARTMENT = "50";
        //public const string PVAT_REMARK = "51";
        public const string FASGRP = "52";
        public const string PCASH_STATUS = "53";
        public const string TAXGRP = "55";
    }
}
