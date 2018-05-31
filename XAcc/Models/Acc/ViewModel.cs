using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XAcc.Models.Secure;

namespace XAcc.Models
{
    //public class ViewMessage
    //{
    //    public string key { get; set; }
    //    public string value { get; set; }
    //    public int int1 { get; set; }
    //}

    public class AddEditResult
    {
        public bool result { get; set; }
        public string message { get; set; }
    }

    public class JstreeJson
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public JstreeJsonState state { get; set; }
        public object a_attr { get; set; }
        public object li_attr { get; set; }
    }

    public class JstreeJsonState
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }

    public class GlaccVM
    {
        public Glacc Glacc { get; set; }
        public string accnum { get { return this.Glacc.accnum; } }
        public string accnam { get { return this.Glacc.accnam; } }
        public string accnam2 { get { return this.Glacc.accnam2; } }
        public string accgrp
        {
            get
            {
                switch (this.Glacc.group)
                {
                    case "1":
                        return "Assets";
                    case "2":
                        return "Liabilities";
                    case "3":
                        return "Shareholder's Equity";
                    case "4":
                        return "Income";
                    case "5":
                        return "Expenses";
                    default:
                        return "**Error**";
                }
            }
        }
        public int level { get { return this.Glacc.level; } }
        public string parent { get { return this.Glacc.parent; } }
        public List<GlaccVM> childacc { get; set; }
    }

    public class ScmodulVM
    {
        public Scmodul Scmodul { get; set; }
        public bool hasChild { get; set; }
    }


    public static class ViewModelHelper
    {
        public static GlaccVM ToGlaccVM(this Glacc glacc, DBAccContext dbacc_context)
        {
            if (glacc == null)
                return null;

            GlaccVM g = new GlaccVM
            {
                Glacc = glacc,
                childacc = dbacc_context.Glacc.Where(a => a.parent == glacc.accnum).ToGlaccVM(dbacc_context)
            };

            return g;
        }

        public static List<GlaccVM> ToGlaccVM(this IEnumerable<Glacc> glacc, DBAccContext dbacc_context)
        {
            List<GlaccVM> g = new List<GlaccVM>();
            foreach (var a in glacc)
            {
                g.Add(a.ToGlaccVM(dbacc_context));
            }

            return g;
        }

        public static ScmodulVM ToScmodulVM(this Scmodul scmodul, DBSecureContext dbsecure_context)
        {
            if (scmodul == null)
                return null;

            ScmodulVM s = new ScmodulVM
            {
                Scmodul = scmodul,
                hasChild = dbsecure_context.Scmodul.Where(sc => sc.parent_modul.Trim() == scmodul.modul.Trim()).Count() > 0 ? true : false
            };

            return s;
        }

        public static List<ScmodulVM> ToScmodulVM(this IEnumerable<Scmodul> scmodul, DBSecureContext dbsecure_context)
        {
            List<ScmodulVM> s = new List<ScmodulVM>();
            foreach (var sc in scmodul)
            {
                s.Add(sc.ToScmodulVM(dbsecure_context));
            }

            return s;
        }
    }
}
