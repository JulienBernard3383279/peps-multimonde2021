using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class PortefeuilleCouverture
    {
        public int Id { get; set; }

        private double deltaEstox;
        private double deltaSp500;
        private double deltaN225;
        private double deltaHang;
        private double deltaFtse;
        private double deltaAsx;

        private double deltaZCEur;
        private double deltaZCUsd;
        private double deltaZCJpy;
        private double deltaZCHkd;
        private double deltaZCGbp;
        private double deltaZCAud;

        public double DeltaEstox { get => deltaEstox; set => deltaEstox = value; }
        public double DeltaSp500 { get => deltaSp500; set => deltaSp500 = value; }
        public double DeltaN225 { get => deltaN225; set => deltaN225 = value; }
        public double DeltaHang { get => deltaHang; set => deltaHang = value; }
        public double DeltaFtse { get => deltaFtse; set => deltaFtse = value; }
        public double DeltaAsx { get => deltaAsx; set => deltaAsx = value; }
        public double DeltaZCEur { get => deltaZCEur; set => deltaZCEur = value; }
        public double DeltaZCUsd { get => deltaZCUsd; set => deltaZCUsd = value; }
        public double DeltaZCJpy { get => deltaZCJpy; set => deltaZCJpy = value; }
        public double DeltaZCHkd { get => deltaZCHkd; set => deltaZCHkd = value; }
        public double DeltaZCGbp { get => deltaZCGbp; set => deltaZCGbp = value; }
        public double DeltaZCAud { get => deltaZCAud; set => deltaZCAud = value; }

        public double getDelta(String nom)
        {
            switch(nom)
            {
                case "estox":
                    return DeltaEstox;
                case "sp500":
                    return DeltaSp500;
                case "n225":
                    return DeltaN225;
                case "hang":
                    return DeltaHang;
                case "ftse":
                    return DeltaFtse;
                case "asx":
                    return DeltaAsx;
                case "eur":
                    return DeltaZCEur;
                case "usd":
                    return DeltaZCUsd;
                case "jpy":
                    return DeltaZCJpy;
                case "hkd":
                    return DeltaZCHkd;
                case "gbp":
                    return DeltaZCGbp;
                case "aud":
                    return DeltaZCAud;
                default:
                    return 0.0;
            }
        }
    }
}