using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models.Secure
{
    //public enum MENU_TYPE : int
    //{
    //    MASTER = 0,
    //    TRANSACTION = 1
    //}

    public class Scmodul
    {
        public int id { get; set; }
        public string modul { get; set; }
        public string desc_th { get; set; }
        public string desc_en { get; set; }
        public string doctyp { get; set; } // transaction menu must specify
        public string parent_modul { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        
    }
}
