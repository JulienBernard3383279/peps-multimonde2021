using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class TauxDeChange
    {
        public String MoneyFrom { get; set; }
        public String MoneyTo { get; set; }
        public Dictionary<DateTime, double> Histo { get; set; }
    }
}