using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class Indice
    {
        #region properties
        [System.ComponentModel.DataAnnotations.Key]
        public DateTime Date { get; private set; }
        public double ASX { get; private set; }
        public double ESTOXX { get; private set; }
        public double FTSE { get; private set; }
        public double HANG { get; private set; }
        public double N225 { get; private set; }
        public double SP500 { get; private set; }



        public double Vol { get; set; }
        public String Money { get; set; }
        public double InterestRateThisArea { get; set; }
        #endregion properties

        public Indice()
        {
        }

        public Indice(DateTime date, double ASX, double ESTOXX, double FTSE, double HANG, double N225, double SP500)
        {
            this.Date = date;
            this.ASX = ASX;
            this.ESTOXX = ESTOXX;
            this.FTSE = FTSE;
            this.HANG = HANG;
            this.N225 = N225;
            this.SP500 = SP500;
            this.Vol = 0.02;
            this.InterestRateThisArea = 0.01;
        }
    }
}