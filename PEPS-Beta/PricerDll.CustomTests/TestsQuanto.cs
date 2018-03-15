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
            double[] correlations,
            double date)
        {

            double d1 = ((Math.Log(currents[0] / strike) + (interestRates[0] + correlations[1] * volatilities[0] * volatilities[1] + 0.5 * volatilities[0] * volatilities[0])) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date));

            double d2 = d1 - volatilities[0] * Math.Sqrt(maturity - date);

            return currents[0] * API.call_pnl_cdfnor(d1) * Math.Exp(-(interestRates[0] - interestRates[1] - correlations[1] * volatilities[0] * volatilities[1]) * (maturity - date)) - strike * Math.Exp(-interestRates[0] * (maturity - date)) * API.call_pnl_cdfnor(d2);

        }


        /*
         * Test pour un ensemble de paramètres donné.
         * Appelle l'API et la formule fermée et compare les résultats.
         * Test actuel : appartenance du résultat à l'intervalle de confiance à 95%.
         * La fonction prend en paramètres les volatilités et covariances de l'actif dans sa monnaie étrangère (S) et du taux de change (X).
         * Les calculs pour envoyer celles de SX et X à l'API sont faites ici. (Temporaires, pour le debug, à changer !)
         */
        private static void PriceTestQuanto(double maturity,
               double strike,
               int nbSamples,
               double[] spots,
               double[] volatilities,
               double[] interestRates,
               double[] correlations)
        {

            double price;
            double ic;

            //DEBUG
            //double[] correlationsModif = MathUtils.GenCorrAPlusBBFromCorrAB(correlations, volatilities);

            API.PriceQuanto(
                maturity, //maturity in years
                strike, //strike when applicable
                nbSamples, //nbSamples
                spots, //spots
                volatilities, //volatilities
                interestRates, //interest rate
                correlations, //correlations
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

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Prix calculé par Monte-Carlo : " + price + " , Intervalle de confiance : [" + (price - ic / 2) + "," + (price + ic / 2) + "]");
            Console.WriteLine("Prix calculé par formule fermée : " + realPrice);
            if ((realPrice > price + ic / 2) || (realPrice < price - ic / 2))
            {
                Console.WriteLine("Vrai prix en dehors de l'intervalle de confiance !");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        /*
         * Lance le test pour certaines combinaisons de valeurs.
         */
        public static void PerformPriceTests()
        {
            double maturity = 3.0;
            double strike = 100.0;
            int nbSamples = 1000000;

            // test sur Call vanille simple
            double[] spots = new double[2] { 100.0, 1.0};
            double[] volatilities = new double[2] { 0.05, 0.00};
            double[] interestRates = new double[2] { 0.05, 0.05 };
            double[] correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            //double[] correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };

            PriceTestQuanto(maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            // test sur Call quanto avec taux de change constant
            spots = new double[2] { 100.0, 0.8 };
            volatilities = new double[2] { 0.05, 0.00 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            //correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };

            PriceTestQuanto(maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            // test sur Call quanto 
            spots = new double[2] { 100.0, 0.8 };
            volatilities = new double[2] { 0.05, 0.05 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            //correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };

            PriceTestQuanto(maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);

            // test sur Call quanto 
            spots = new double[2] { 100.0, 0.8 };
            volatilities = new double[2] { 0.05, 0.02 };
            interestRates = new double[2] { 0.05, 0.05 };
            correlations = new double[4] { 1.0, 0.1, 0.1, 1.0 };
            //correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };

            PriceTestQuanto(maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations);
        }

        private static double[] RealDeltaQuanto0(
                double maturity,
                double strike,
                double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
                double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
                double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
                double[] correlations,
                double[] FXRates,//une seule
                double date)
        {
            double d1 = (Math.Log(currents[0] / strike) + (interestRates[0] + correlations[0] * volatilities[0] * volatilities[1] + 0.5 * volatilities[0] * volatilities[0])) / (volatilities[0] * Math.Sqrt(maturity));
            double d2 = d1 - volatilities[0] * Math.Sqrt(maturity);
            double[] deltas = new double[2] { Math.Exp(-(interestRates[0] - interestRates[1] - correlations[0] * volatilities[0] * volatilities[1]) * (maturity - date)) * API.call_pnl_cdfnor(d1) * (1 / FXRates[1]), Math.Exp(interestRates[0] * maturity) * currents[0] * Math.Exp(-(interestRates[0] - interestRates[1] - correlations[0] * volatilities[0] * volatilities[1]) * (maturity)) * API.call_pnl_cdfnor(d1) - strike * API.call_pnl_cdfnor(d2) };//1/FXRates c'est le prix d'un euro en dollars  
            return deltas;
        }
        private static double[] RealDeltaQuantoAnyTime(
                double maturity,
                double strike,
                double[] currents,//on le veut (l'actif) dans la monnaie etrangère,sa monnaie de base quoi ici.Tableau de taille 1.
                double[] volatilities,//les vol dans un ordre suivant: actif puis taux de change de 1euro en dollars
                double[] interestRates,//les taux d'interets domestiques et etrangers dans cet ordre!
                double[] correlations,
                double[] FXRates,//une seule
                double date)
        {
            double d1 = (Math.Log(currents[0] / strike) + (interestRates[0] + correlations[0] * volatilities[0] * volatilities[1] + 0.5 * volatilities[0] * volatilities[0])) / (volatilities[0] * Math.Sqrt(maturity - date));
            double d2 = d1 - volatilities[0] * Math.Sqrt(maturity - date);
            double[] deltas = new double[2] { Math.Exp(-(interestRates[0] - interestRates[1] - correlations[0] * volatilities[0] * volatilities[1]) * (maturity - date)) * API.call_pnl_cdfnor(d1) * (1 / FXRates[1]), Math.Exp(interestRates[0] * (maturity - date)) * currents[0] * Math.Exp(-(interestRates[0] - interestRates[1] - correlations[0] * volatilities[0] * volatilities[1]) * (maturity - date)) * API.call_pnl_cdfnor(d1) - strike * API.call_pnl_cdfnor(d2) };//1/FXRates c'est le prix d'un euro en dollars  
            return deltas;
        }



        private static void DeltaTest0(double maturity,
               int optionSize,
               double strike,
               double[] payoffCoefficients,
               int nbSamples,
               double[] spots,
               double[] volatilities,
               double[] interestRates,
               double[] correlations,
               int timestepNumber,
               double[] FXRates,
               double[] trends)
        {


            //call quanto = une seule monnaie pour l'actif (un actif quoi), elle est etrangère
            API.DeltasSingleCurrencyBasket(
                maturity,
                optionSize,
                strike,
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

            double[] realDelta = RealDeltaQuanto0(maturity,
                strike,
                spots,
                volatilities,
               interestRates,
                correlations,
                FXRates,
                date);
            double[] deltas = new double[6];
            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, deltas, 0, 6);

            if (Math.Abs((realDelta[0] - deltas[0]) / deltas[0]) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème de deltas pour l'option quanto en t=0!");
            }
        }

        private static void DeltaTestAnyTime(double maturity,
              int optionSize,
              double strike,
              double[] payoffCoefficients,
              int nbSamples,
              double[] spots,
              double[] volatilities,
              double[] interestRates,
              double[] correlations,
              int timestepNumber,
              double[] FXRates,
              double[] trends)
        {


            //call quanto = une seule monnaie pour l'actif (un actif quoi), elle est etrangère
            API.DeltasSingleCurrencyBasket(
                maturity,
                optionSize,
                strike,
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

            double[] realDelta = RealDeltaQuantoAnyTime(maturity,
                strike,
                spots,
                volatilities,
               interestRates,
                correlations,
                FXRates,
                date);
            double[] deltas = new double[6];
            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, deltas, 0, 6);

            if (Math.Abs((realDelta[0] - deltas[0]) / deltas[0]) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème de deltas pour l'option quanto en t>0!");
            }
        }

        public static void PerformDeltaTests0()
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
            double[] FXRates = new double[1] { 0.85 };

            DeltaTest0(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                timestepNumber,
                FXRates,
                trends);
        }
        public static void PerformDeltaTestsAnyTime()
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
            double[] FXRates = new double[1] { 0.85 };

            DeltaTestAnyTime(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                timestepNumber,
                FXRates,
                trends);
        }
    }
}


