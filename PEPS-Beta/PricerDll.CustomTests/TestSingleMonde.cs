using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestSingleMonde
    {
        private static double RealPriceSingleMonde(
            double maturity,
            double S0,
            double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
            double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
            double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
            double[] correlations,
            double date)
        {

            double d1 = ((Math.Log(currents[0] / 1.15 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));
            double d2 = ((Math.Log(currents[0] / 0.85 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));


            return (currents[0] * API.call_pnl_cdfnor(d1) * (1.15 * S0 + currents[0]) + (0.85) * S0 - currents[0] +
                (currents[0] - (0.85) * S0) * API.call_pnl_cdfnor(d2)) * Math.Exp(-(interestRates[0]) * (maturity - date));

        }

        /* ensemble de tests vis à vis du singlemonde*/
        /* private static void PriceTestQuanto(double maturity,
                double strike,
                int nbSamples,
                double[] spots,
                double[] volatilities,
                double[] interestRates,
                double[] correlations)
         {


         }*/
    }
    }
