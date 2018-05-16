﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
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
    }
}