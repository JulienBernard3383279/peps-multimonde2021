using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class Indice
    {
        public int id { get; set; }
        public String Nom { get; }
        public double Vol { get; set; }
        public Dictionary<DateTime, double> Histo { get; set; }
        public Dictionary<Indice, double> CorrelationMat { get; set; }
    }
}