using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models.Secure
{
    public class Scuser
    {
        public int ID { get; set; }
        public string rectyp { get; set; }
        public string reccod { get; set; }
        public string connectgrp { get; set; }
        public string recdes { get; set; }
        public DateTime? revokedat { get; set; }
        public DateTime? resumedat { get; set; }
        public string language { get; set; }
        public int userattr { get; set; }
        public DateTime? laswrk { get; set; }
        public DateTime? laspwd { get; set; }
        public DateTime? lasusedat { get; set; }
        public string lasusetim { get; set; }
        public string secure { get; set; }
        public int authlev { get; set; }
        public string userpwd { get; set; }
        public string status { get; set; }
        public string prnnum { get; set; }
        public string rwttxt { get; set; }

    }
}
