using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class Indice
    {
        public int Id { get; set; }
        public String Nom { get; set; }
        public double Vol { get; set; }
        // Correlation Indexes
        public double corrESTOXX { get; set; }
        public double corrSP500 { get; set; }
        public double corrN225 { get; set; }
        public double corrHANG { get; set; }
        public double corrFTSE { get; set; }
        public double corrAUD { get; set; }
        // Correlation change
        public double corrEURUSD { get; set; }
        public double corrEURJPY { get; set; }
        public double corrEURHKD { get; set; }
        public double corrEURGBP { get; set; }
        public double corrEURAUD { get; set; }

        public String Money { get; set; }
        public double InterestRateThisArea { get; set; }

        public Indice()
        {
        }

        public Indice(String name, String mon)
        {
            this.Nom = name;
            this.Money = mon;
            this.Vol = 0.02;
            this.InterestRateThisArea = 0.01;
        }
    }
}