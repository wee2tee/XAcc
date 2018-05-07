using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models.MainDB
{
    public class Customer
    {
        public int id { get; set; }
        public string sernum { get; set; }
        public string prenam { get; set; }
        public string compnam { get; set; }
        public string taxid { get; set; }
        public string orgnum { get; set; }
        public string server_name { get; set; }
        public int server_port { get; set; }
        public string mysql_uid { get; set; }
        public string mysql_pwd { get; set; }
        public string pass_phrase { get; set; }
    }
}
