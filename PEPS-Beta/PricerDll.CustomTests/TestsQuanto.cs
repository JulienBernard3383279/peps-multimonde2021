using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestsQuanto
    {
        private static double RealPriceQuanto(
            double maturity,
            double strike,
            double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
            double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
            double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
            double[] correlations,//une seule
            double date)
        {
            double d1 = (Math.Log(currents[0] / strike) + (interestRates[0] + correlations[0] * volatilities[0] * volatilities[1] + 0.5 * volatilities[0] * volatilities[0])) / (volatilities[0] * Math.Sqrt(maturity - date));
            double d2 = d1 - volatilities[0] * Math.Sqrt(maturity - date);
            return currents[0] * API.call_pnl_cdfnor(d1) * Math.Exp(-(interestRates[0] - interestRates[1] - correlations[0] * volatilities[0] * volatilities[1]) * (maturity - date)) - strike * Math.Exp(-interestRates[0] * (maturity - date)) * API.call_pnl_cdfnor(d2);

        }



        private static void PriceTest(double maturity,
               int optionSize,
               double strike,
               double[] payoffCoefficients,
               int nbSamples,
               double[] spots,
               double[] volatilities,
               double[] interestRates,
               double[] correlations,
               int timestepNumber,
               double[] trends)
        {

            double price;
            double ic;


            API.PriceBasket(
                maturity, //maturity in years
                optionSize, //optionSize
                strike, //strike when applicable
                payoffCoefficients, //payoffCoefficients
                nbSamples, //nbSamples
                spots, //spots
                volatilities, //volatilities
                interestRates[0], //interest rate
                correlations, //correlations
                timestepNumber,
                trends, //trends (donc égaux au taux d'intérêt)
                &price,
                &ic);
           
            double date = 0.0; 
            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = RealPriceQuanto(maturity,
                strike,
                spots,
                volatilities,
               interestRates,
                correlations,
                date);

            if (Math.Abs((realPrice - price) / price) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème !");
            }
        }

        public static void PerformPriceTests()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 100000;
            double[] spots = new double[1] { 1.0 };
            double[] volatilities = new double[1] { 1.0 };
            double[] interestRates = new double[2] { 0.05, 0.0 };
            double[] correlations = new double[1] { 1.0 };
            int timestepNumber = 1;
            double[] trends = new double[1] { 1.0 };

            PriceTest(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                timestepNumber,
                trends);
        }
    }
}
