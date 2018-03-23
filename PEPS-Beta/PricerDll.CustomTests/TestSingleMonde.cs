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
            double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
            double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
            double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
            double[] correlations,
            double date)
        {
            double S0 = currents[0];
            double d1 = ((Math.Log(currents[0] / 1.15 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));
            double d2 = ((Math.Log(currents[0] / 0.85 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));


            return 0;// ( * API.call_pnl_cdfnor(d1) * (1.15 * S0 + currents[0]) + (0.85) * S0 - currents[0] +
               // (currents[0] - (0.85) * S0) * API.call_pnl_cdfnor(d2)) * Math.Exp(-(interestRates[0]) * (maturity - date));

        }

        /* ensemble de tests vis à vis du singlemonde*/

        private static void PriceTestSingleMonde(int sampleNumber,
    //double past[], // format [,]
    double[] currentPrices,//taille 1, il s'agit juste du spot
    double[] volatilities,//taille 1 pareil
    double[] interestRates,//pour l'instant taille 1
    double maturity)
        {

            double price_;
            double ic_;


            API.PriceSingleMonde(sampleNumber,
      //double past[], // format [,]
      currentPrices,//taille 1, il s'agit juste du spot
      volatilities,//taille 1 pareil
      interestRates,//pour l'instant taille 1
      &price_,
      maturity,
      &ic_);

            double date = 0.0;
            //price et ics contiennent prix et intervalle de couverture selon le pricer
            double[] correlations = new double[1];
            correlations[0] = 0.0;
            double realPrice = RealPriceSingleMonde(
                maturity,
             currentPrices,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
             volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
             interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
            correlations,
             date);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Prix calculé par Monte-Carlo : " + price_ + " , Intervalle de confiance à 99% : [" + (price_ - 1.5 * ic_ / 2) + "," + (price_ + 1.5 * ic_ / 2) + "]");
            Console.WriteLine("Prix calculé par formule fermée : " + realPrice);
            if ((realPrice > price_ + 1.5 * ic_ / 2) || (realPrice < price_ - 1.5 * ic_ / 2))
            {
                Console.WriteLine("Vrai prix en dehors de l'intervalle de confiance !");
            }
            Console.WriteLine("");
            Console.WriteLine("");

        }
        /*
        * Lance le test pour certaines combinaisons de valeurs.
        */
        public static void PerformPriceSingleMondeTests()
        {
            double maturity = 3.0;
            int nbSamples = 1000000;

            Console.WriteLine("Test sur singlemonde une date, un actif, marché euro");
            double[] spots = new double[1] { 100.0 };
            double[] volatilities = new double[1] { 0.05 };
            double[] interestRates = new double[1] { 0.05 };
            double[] correlations = new double[1] { 0.0 };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                maturity);

        }

        private static double RealDeltaSingleMonde(
           double maturity,
           double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
           double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
           double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
           double[] correlations,
           double date)
        {
            double S0 = currents[0];
            double d1 = ((Math.Log(currents[0] / 1.15 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));
            double d2 = ((Math.Log(currents[0] / 0.85 * S0) + (interestRates[0] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));


            return ( API.call_pnl_cdfnor(d1)  + -1 + API.call_pnl_cdfnor(d2)) * Math.Exp(-(interestRates[0]) * (maturity - date));

        }

        private static void DeltaTestSingleMonde(double maturity,
               int nbSamples,
               double[] spots,
               double[] volatilities,
               double[] interestRates,
               double[] correlations,
               int timestepNumber,
               double[] FXRates,
               double[] trends)
        {
/*

            //singlemonde = une seule monnaie pour l'actif
            API.DeltasSingleCurrencyBasket(
                maturity,
                1,
                0.0,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRates[0],
                correlations,
                trends,
                FXRates,
                out IntPtr deltasAssets,
                out IntPtr deltasFXRates);

            double date = 0.0;
            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double[] realDelta = RealDeltaSingleMonde(maturity,
                spots,
                volatilities,
               interestRates,
                correlations,
                date);
            double[] deltas = new double[1];
            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, deltas, 0, 1);

            if (Math.Abs((realDelta[0] - deltas[0]) / deltas[0]) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème de deltas pour l'option quanto en t=0!");
            }*/
        }
    }
}
