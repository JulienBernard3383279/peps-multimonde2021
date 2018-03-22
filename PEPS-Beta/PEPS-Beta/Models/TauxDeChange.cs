using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class TauxDeChange
    {
        public int Id { get; set; }
        public String MoneyFrom { get; set; }
        public String MoneyTo { get; set; }
        public Dictionary<DateTime, double> Histo { get; set; }

        public TauxDeChange(String From, String To, Dictionary<DateTime,double> histo)
        {
            this.MoneyFrom = From;
            this.MoneyTo = To;
            this.Histo = new Dictionary<DateTime, double>(histo);
        }
    }
}