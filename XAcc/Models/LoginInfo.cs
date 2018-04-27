using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class LoginInfo
    {
        public string sernum { get; set; }
        public string phrase { get; set; }
        public string username { get; set; }
        public string pass { get; set; }
        public bool login_result { get; set; }
    }
}
