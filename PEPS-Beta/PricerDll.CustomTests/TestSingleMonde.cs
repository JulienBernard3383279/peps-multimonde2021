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


            return (currents[0] * API.call_pnl_cdfnor(d1) * (1.15 * S0 + currents[0]) + (0.85) * S0 - currents[0] +
                (currents[0] - (0.85) * S0) * API.call_pnl_cdfnor(d2)) * Math.Exp(-(interestRates[0]) * (maturity - date));

        }

        /* ensemble de tests vis à vis du singlemonde*/

        private static void PriceTestSingleMonde(int sampleNumber,
    //double past[], // format [,]
    double[] currentPrices,//taille 1, il s'agit juste du spot
    double[] volatilities,//taille 1 pareil
    double[] interestRates,//pour l'instant taille 1
    double* price,
    double maturity,
    double* ic)
        {

            double price_;
            double ic_;


           API.PriceSingleMonde( sampleNumber,
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
            Console.WriteLine("Prix calculé par Monte-Carlo : " + price_ + " , Intervalle de confiance à 99% : [" + (price - 1.5 * ic / 2) + "," + (price + 1.5 * ic / 2) + "]");
            Console.WriteLine("Prix calculé par formule fermée : " + realPrice);
            if ((realPrice > price_ + 1.5 * ic_ / 2) || (realPrice < price_ - 1.5 * ic_ / 2))
            {
                Console.WriteLine("Vrai prix en dehors de l'intervalle de confiance !");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        
    }
    }
}
