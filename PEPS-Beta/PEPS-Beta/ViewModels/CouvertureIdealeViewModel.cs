using PEPS_Beta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.ViewModels
{
    public class CouvertureIdealeViewModel
    {
        public DateTime CurrDate { get; set; }
        public PortefeuilleCouverture IdealPort { get; set; }
    }
}