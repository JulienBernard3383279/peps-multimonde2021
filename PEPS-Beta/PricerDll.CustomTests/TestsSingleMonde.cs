using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestsSingleMonde
    {

        /*private static double RealPriceSingleMonde(
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

            return (API.call_pnl_cdfnor(d1) * (1.15 * S0 + currents[0]) + (0.85) * S0 - currents[0] +
                (currents[0] - (0.85) * S0) * API.call_pnl_cdfnor(d2)) * Math.Exp(-(interestRates[0]) * (maturity - date));
        } Renvoie 215. Lol.*/

        private static double RealPriceSingleMonde(
            double maturity,
            double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
            double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
            double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
            double[] correlations,
            double date)
        {
            return 85*Math.Exp(-interestRates[0] * (maturity-date))
                + 100/currents[0] * TestsQuanto.RealPriceCallQuanto(maturity, 0.85 * currents[0], currents, volatilities, interestRates, correlations, date)
                - 100/currents[0] * TestsQuanto.RealPriceCallQuanto(maturity, 1.15 * currents[0], currents, volatilities, interestRates, correlations, date);
        }


        /* ensemble de tests vis à vis du singlemonde*/

        private static void PriceTestSingleMonde(
            int sampleNumber,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations)
        {
            double price_;
            double ic_;

            double date = 0.0;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            API.PriceSingleMonde(
                sampleNumber,
                currentPrices,
                volatilities,
                interestRates,
                correlations,
                date,
                currentPrices,
                &price_,
                &ic_);
            watch.Stop();
            var executionTime = watch.ElapsedMilliseconds;

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = RealPriceSingleMonde(
                371.0/365.25,
                currentPrices,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
                volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
                interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
                correlations,
                date);

            Console.WriteLine("");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix calculé par Monte-Carlo : " + price_ + " , Intervalle de confiance à 99% : [" + (price_ - 1.5 * ic_ / 2) + "," + (price_ + 1.5 * ic_ / 2) + "]");
            Console.WriteLine("Prix calculé par formule fermée : " + realPrice);
            if ((realPrice > price_ + 1.5 * ic_ / 2) || (realPrice < price_ - 1.5 * ic_ / 2))
            {
                Console.WriteLine("Vrai prix en dehors de l'intervalle de confiance !");
            }
            Console.WriteLine("");
        }
        /*
        * Lance le test pour certaines combinaisons de valeurs.
        */
        public static void PerformPriceTests()
        {
            int nbSamples = 1_000_000;

            double currFXRate;
            double[] spots;
            double[] volatilities;
            double[] interestRates;
            double[] correlations;

            Console.WriteLine("Monde gelé");
            currFXRate = 1;
            volatilities = new double[2] { 0.0, 0.0 };
            interestRates = new double[2] { 0.0, 0.0 };
            correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0/365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Volatilité de l'actif = 2%");
            volatilities = new double[2] { 0.02, 0.0 };
            interestRates = new double[2] { 0.0, 0.0 };
            correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Taux d'intérêt Euro = 5%");
            volatilities = new double[2] { 0.02, 0.0 };
            interestRates = new double[2] { 0.05, 0.0 };
            correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Taux d'intérêt étranger = 5%");
            volatilities = new double[2] { 0.02, 0 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Volatilité du taux de change = 5%");
            volatilities = new double[2] { 0.02, 0.02 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Corrélation 0.1");
            volatilities = new double[2] { 0.02, 0.02 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Spots 120.0");
            volatilities = new double[2] { 0.02, 0.02 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            spots = new double[2] { 120.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("+ Taux de change 0.5");
            currFXRate = 0.5;
            volatilities = new double[2] { 0.02, 0.02 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            spots = new double[2] { 120.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("Valeurs random");
            currFXRate = 10;
            volatilities = new double[2] { 0.05, 0.06 };
            interestRates = new double[2] { 0.2, 0.01 };
            correlations = new double[4] { 1.0, -0.1, -0.1, 1.0 };
            spots = new double[2] { 150.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            Console.WriteLine("Valeurs random");
            currFXRate = 0.3;
            volatilities = new double[2] { 0.03, 0.01 };
            interestRates = new double[2] { -0.01, 0.04 };
            correlations = new double[4] { 1.0, 0.15, 0.15, 1.0 };
            spots = new double[2] { 10.0, currFXRate * Math.Exp(-interestRates[1] * 371.0 / 365.25) };
            PriceTestSingleMonde(
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);
        }
        
        private static double[] RealDeltaSingleMonde(
           double maturity,
           double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
           double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
           double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
           double[] correlations,
           double date)
        {
            double[] deltas = new double[2];
            deltas[0] = 100 / currents[0] * (TestsQuanto.RealDeltaQuanto0(maturity, 0.85 * currents[0], new double[1] { currents[0] }, volatilities, interestRates, correlations, new double[1] { currents[1] }, date)[0]
                -  TestsQuanto.RealDeltaQuanto0(maturity, 1.15 * currents[0], new double[1] { currents[0] }, volatilities, interestRates, correlations, new double[1] { currents[1] }, date)[0]);
            deltas[1] = 100 / currents[0] * (TestsQuanto.RealDeltaQuanto0(maturity, 0.85 * currents[0], new double[1] { currents[0] }, volatilities, interestRates, correlations, new double[1] { currents[1] }, date)[1]
                            - TestsQuanto.RealDeltaQuanto0(maturity, 1.15 * currents[0], new double[1] { currents[0] }, volatilities, interestRates, correlations, new double[1] { currents[1] }, date)[1]);
            return deltas;
        }

        private static void DeltaTestSingleMonde(double maturity,
               double strike,
               int nbSamples,
               double[] spots,
               double[] volatilities,
               double[] interestRates,
               double[] correlations,
               double currFXRate)
        {
            //call quanto = une seule monnaie pour l'actif (un actif quoi), elle est etrangère
            API.DeltasSingleMonde(
                maturity,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                0,
                spots,
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

            double[] deltas = new double[2];
            double[] tmp = new double[1];

            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, tmp, 0, 1);
            deltas[0] = tmp[0];
            System.Runtime.InteropServices.Marshal.Copy(deltasFXRates, tmp, 0, 1);
            deltas[1] = tmp[0];

            double realPrice = RealPriceSingleMonde(maturity, spots, volatilities, interestRates, correlations, 0);
            double tmpD = realPrice - realDelta[0] * spots[0] * currFXRate - realDelta[1] * Math.Exp(-interestRates[0] * maturity);
            tmpD /= spots[1];
            realDelta[1] = tmpD;

            if (Math.Abs((realDelta[0] - deltas[0]) / deltas[0]) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème de deltas pour l'option quanto en t=0!");
                Console.WriteLine("Deltas formule fermée:");
                Console.WriteLine(realDelta[0]);
                Console.WriteLine(realDelta[1]);
                Console.WriteLine("Deltas simulés");
                Console.WriteLine(deltas[0]);
                Console.WriteLine(deltas[1]);
            }
            else
            {
                Console.WriteLine("Deltas formule fermée:");
                Console.WriteLine(realDelta[0]);
                Console.WriteLine(realDelta[1]);
                Console.WriteLine("Deltas simulés");
                Console.WriteLine(deltas[0]);
                Console.WriteLine(deltas[1]);
            }
        }

        public static void PerformDeltaTests()
        {
            /*double maturity = 3.0;
            double strike = 100.0;
            int nbSamples = 10000;
            double currFXRate = 1.2;
            double[] interestRates = new double[2] { 0.05, 0.03 }; ;
            double[] spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * maturity) };
            double[] volatilities = new double[2] { 0.01, 0.02 };
            double[] correlations = new double[4] { 1.0, 0.05, 0.05, 1.0 };

            double realPrice = RealPriceSingleMonde(maturity, spots, volatilities, interestRates, correlations, 0);
            Console.WriteLine("Prix fermé " + realPrice);
            DeltaTestSingleMonde(maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                currFXRate);*/
        }
    }
    
}
