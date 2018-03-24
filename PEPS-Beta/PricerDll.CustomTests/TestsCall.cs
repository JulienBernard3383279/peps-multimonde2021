using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PricerDll.CustomTests
{
    
    public static unsafe class TestsCall
    {
        
        public static double RealPrice(
            double maturity,
            double strike,
            double[] currents,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double date) 
        {
            double d1 = ( Math.Log(currents[0] / strike) + (interestRate + volatilities[0]*volatilities[0]*0.5)*(maturity - date) ) / (volatilities[0] *Math.Sqrt (maturity - date));
            double d2 = d1 - volatilities[0] * Math.Sqrt(maturity - date);
            return currents[0] * API.call_pnl_cdfnor(d1) - strike * Math.Exp(-interestRate * (maturity - date) )* API.call_pnl_cdfnor(d2);
        }

        private static void PriceTest(double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int nbSamples,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
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
                interestRate, //interest rate
                correlations, //correlations
                trends, //trends (donc égaux au taux d'intérêt)
                &price,
                &ic);

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = RealPrice(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                0.0);
            // directive assert : Assure que le prix renvoyé par la formule fermée et celui renvoyé par la simulation sont dans un intervalle de confiance de largeur 2%.
            Assert.IsTrue(Math.Abs((realPrice - price) / price) < 0.02);

           /* Console.WriteLine("Prix selon la formule : " + realPrice);
            Console.WriteLine("Prix selon le pricer : " + price);
            if (Math.Abs( (realPrice-price)/price) > 0.02) {
                Console.WriteLine("Test du prix du call en 0 : différence de prix supérieur à 2%.");
            }
            else
            {
                Console.WriteLine("Test du prix du call en 0 concluant.");
            }
            Console.WriteLine();*/
        }

        // lance le test ci-dessus à partir des données de test ci-dessous 
        public static void PerformPriceTests()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] spots = new double[1] { 100.0 };
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };

            PriceTest(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRate,
                correlations,
                trends);
        }






        private static void PriceTestAnyTime(double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int nbSamples,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends)
        {
            double price;
            double ic;

            API.PriceBasketAnyTime(
                maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                past,
                nbRows,
                t,
                current,
                volatilities,
                interestRate,
                correlations,
                trends,
                &price,
                &ic);

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = RealPrice(maturity,
                strike,
                current,
                volatilities,
                interestRate,
                correlations,
                t);
            //assure que prix formule fermée et prix simulé en tout t<T  dans un intervalle de confiance de largeur 2%
            Assert.IsTrue(Math.Abs((realPrice - price) / price) < 0.02);
           /* Console.WriteLine("Prix selon la formule : " + realPrice);
            Console.WriteLine("Prix selon le pricer : " + price);
            if (Math.Abs((realPrice - price) / price) > 0.02)
            {
                Console.WriteLine("Test du prix du call en t : différence de prix supérieur à 2%.");
            }
            else
            {
                Console.WriteLine("Test du prix du call en t concluant.");
            }
            Console.WriteLine();
            */

        }

        public static void PerformPriceTestsAnyTime()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] past = new double[1] { 100.0 };
            int nbRows = 1;
            double[] currents = new double[1] { 105.0 };
            double t = 1.0;
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };

            PriceTestAnyTime(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                past,
                nbRows,
                t,
                currents,
                volatilities,
                interestRate,
                correlations,
                trends);
        }









        //Delta 0

        private static double RealDelta0(
               double maturity,
               double strike,
               double[] currents,
               double[] volatilities,
               double interestRate,
               double[] correlations,
               double date)
        {
            double d1 = (Math.Log(currents[0] / strike) + (interestRate + volatilities[0] * volatilities[0] * 0.5)*(maturity)) / (volatilities[0] * Math.Sqrt(maturity));
            return API.call_pnl_cdfnor(d1);
        }

        private static void DeltaTest0(double maturity,
                int optionSize,
                double strike,
                double[] payoffCoefficients,
                int nbSamples,
                double[] spots,
                double[] volatilities,
                double interestRate,
                double[] correlations,
                int timestepNumber,
                double[] trends)
        {

            API.DeltasMultiCurrencyBasket(
                 maturity,
                 optionSize,
                 strike,
                 payoffCoefficients,
                 nbSamples,
                 spots,
                 volatilities,
                 interestRate,
                 correlations,
                 trends,
                 out IntPtr deltasPtr);

            double[] deltas = new double[1];
            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 1); //< -deltas contient maintenant les deltas
            
            double realDelta = RealDelta0(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                0.0);
            //Assure que le delta calculé et simulé sont dans un intervalle de confiance de largeur inferieure à 2%.
            Assert.IsTrue(Math.Abs((realDelta - deltas[0]) / deltas[0]) < 0.02);
            /*
            Console.WriteLine("Delta selon la formule : " + realDelta);
            Console.WriteLine("Delta selon le pricer : " + deltas[0]);
            //on teste juste le delta de l'option ,rien de plus,et en 0
            if (Math.Abs( (realDelta - deltas[0]) / deltas[0] ) > 0.02)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("Test du delta du call en 0 : différence de delta supérieur à 2%.");
            }
            else
            {
                Console.WriteLine("Test du delta du call en 0 concluant.");
            }
            Console.WriteLine();
            */
        }

        public static void PerformDeltaTests0()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] spots = new double[1] { 100.0 };
            double[] volatilities = new double[1] { 0.1 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            int timestepNumber = 1;
            double[] trends = new double[1] { 0.05 };

            DeltaTest0(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRate,
                correlations,
                timestepNumber,
                trends);
        }











        private static double RealDeltaAnyTime(
            double maturity,
            double strike,
            double[] currents,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double date)
        {
            double d1 = ((Math.Log(currents[0] / strike) + (interestRate + volatilities[0] * volatilities[0] * 0.5) * (maturity - date)) / (volatilities[0] * Math.Sqrt(maturity - date)));
            return API.call_pnl_cdfnor(d1);
        }

        private static void DeltaTestAnyTime(double maturity,
                int optionSize,
                double strike,
                double[] payoffCoefficients,
                int nbSamples,
                double[] volatilities,
                double interestRate,
                double[] correlations,
                double[] past,
                int nbRows,
                double[] current,
                double t,
                double[] trends)
        {
            double[] deltas = new double[1];
            API.DeltasMultiCurrencyBasketAnyTime(
                 maturity,
                 optionSize,
                 strike,
                 payoffCoefficients,
                 nbSamples,
                 past,
                 nbRows,
                 t,
                 current,
                 volatilities,
                 interestRate,
                 correlations,
                 trends,
                 out IntPtr deltasPtr);

            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 1); //< -deltas contient maintenant les deltas

            double realDelta = RealDeltaAnyTime(maturity,
                strike,
                current,
                volatilities,
                interestRate,
                correlations,
                t);
            Assert.IsTrue(Math.Abs((realDelta - deltas[0]) / deltas[0]) < 0.02);
            /*
            Console.WriteLine("Delta selon la formule : " + realDelta);
            Console.WriteLine("Delta selon le pricer : " + deltas[0]);
            if (Math.Abs((realDelta - deltas[0]) / deltas[0]) > 0.02)
            {
                Console.WriteLine("Test du delta du call en t : différence de delta supérieur à 2%.");
            }
            else
            {
                Console.WriteLine("Test du delta du call en t concluant.");
            }
            Console.WriteLine();
            */
        }

        public static void PerformDeltaTestsAnyTime()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };
            double t = 1.0;
            double[] past = new double[1] { 100.00 };
            int nbRows = 1;
            double[] currents = new double[1] { 95.0 };

            DeltaTestAnyTime(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                volatilities,
                interestRate,
                correlations,
                past,
                nbRows,
                currents,
                t,
                trends);
        }
    }
}
