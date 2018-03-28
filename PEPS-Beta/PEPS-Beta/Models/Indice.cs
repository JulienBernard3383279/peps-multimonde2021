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
        public double MoneyVol { get; set; }
        // Correlation Indexes
        public double corrESTOXX { get; set; }
        public double corrSP500 { get; set; }
        public double corrN225 { get; set; }
        public double corrHANG { get; set; }
        public double corrFTSE { get; set; }
        public double corrASX { get; set; }
        // Correlation change
        public double corrEURUSD { get; set; }
        public double corrEURJPY { get; set; }
        public double corrEURHKD { get; set; }
        public double corrEURGBP { get; set; }
        public double corrEURAUD { get; set; }
        public String Money { get; set; }
        public double InterestRateThisArea { get; set; }
        public double corrTDCESTOXX { get; internal set; }
        public double corrTDCSP500 { get; internal set; }
        public double corrTDCN225 { get; internal set; }
        public double corrTDCHANG { get; internal set; }
        public double corrTDCFTSE { get; internal set; }
        public double corrTDCASX { get; internal set; }
        public double corrTDCEURUSD { get; internal set; }
        public double corrTDCEURJPY { get; internal set; }
        public double corrTDCEURHKD { get; internal set; }
        public double corrTDCEURGBP { get; internal set; }
        public double corrTDCEURAUD { get; internal set; }

        public double[] getCorrI()
        {
            return new double[11] { corrESTOXX, corrSP500 , corrN225, corrHANG, corrFTSE, corrASX, corrEURUSD, corrEURJPY, corrEURHKD, corrEURGBP, corrEURAUD };
        }

        public double[] getCorrTDC()
        {
            return new double[11] { corrTDCESTOXX, corrTDCSP500, corrTDCN225, corrTDCHANG, corrTDCFTSE, corrTDCASX, corrTDCEURUSD, corrTDCEURJPY, corrTDCEURHKD, corrTDCEURGBP, corrTDCEURAUD };
        }

        public Indice()
        {
        }

        public Indice(String name, String mon, double interestRate)
        {
            this.Nom = name;
            this.Money = mon;
            this.Vol = 0.02;
            this.MoneyVol = 0.02;
            this.InterestRateThisArea = interestRate;
        }
    }
}