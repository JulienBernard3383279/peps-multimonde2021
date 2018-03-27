using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class TauxDeChange
    {
        [System.ComponentModel.DataAnnotations.Key]
        public DateTime date { get; private set; }
        public double EURUSD { get; private set; }
        public double EURAUD { get; private set; }
        public double EURGBP { get; private set; }
        public double EURJPY { get; private set; }
        public double EURHKD { get; private set; }

        public TauxDeChange(DateTime date, double EURUSD, double EURAUD, double EURGBP, double EURJPY, double EURHKD)
        {
            this.date = date;
            this.EURAUD = EURAUD;
            this.EURUSD = EURUSD;
            this.EURGBP = EURGBP;
            this.EURJPY = EURJPY;
            this.EURHKD = EURHKD;
        }
    }
}