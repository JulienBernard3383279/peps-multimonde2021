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
        public Dictionary<DateTime, double> Histo { get; set; }
        public Dictionary<Indice, double> CorrelationMat { get; set; }
        public String Money { get; set; }
        public double InterestRateThisArea { get; set; }

        public Indice()
        {
        }

        public Indice(String name)
        {
            this.Nom = name;
            this.Vol = 0.02;
            this.Histo = new Dictionary<DateTime, double>();
            this.CorrelationMat = new Dictionary<Indice, double>();
        }
    }
}