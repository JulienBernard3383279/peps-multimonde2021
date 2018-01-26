using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestsCall
    {
        private static double RealPrice(
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


            Console.WriteLine(realPrice);
            Console.WriteLine(price);
            if (Math.Abs( (realPrice-price)/price) > 0.02) {
                Console.WriteLine("Test du prix du call : différence de prix supérieur à 2%.");
            }
            else
            {
                Console.WriteLine("Test du prix du call concluant.");

            }
        }

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
            int timestepNumber = 1;
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
                timestepNumber,
                trends);
        }

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


        private static double RealDeltaAnyTime(
              double maturity,
              double strike,
              double[] currents,
              double[] volatilities,
              double interestRate,
              double[] correlations,
              double date)
        {

            double d1 = ((Math.Log(currents[0] / strike) + (interestRate + volatilities[0] * volatilities[0] * 0.5)*(maturity-date)) / (volatilities[0] * Math.Sqrt(maturity-date)));
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

            double[] deltas = new double[6];
            double[] FXRates = new double[6];
            API.DeltasSingleCurrencyBasket(
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
                  FXRates,
                 out IntPtr deltasAssets,
                 out IntPtr deltasFXRates);

            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, deltas, 0, 6); //< -deltas contient maintenant les deltas
            
            double realDelta = RealDelta0(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                0.0);
            Console.WriteLine(realDelta);
            Console.WriteLine(deltas[0]);
            //on teste juste le delta de l'option ,rien de plus,et en 0
            if (Math.Abs( (realDelta - deltas[0]) / deltas[0] ) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("le delta du call calculé en 0 par le pricer est faux !");
            }
        }

        private static void DeltaTestAnyTime(double maturity,
                int optionSize,
                double strike,
                double[] payoffCoefficients,
                int nbSamples,
                double[] spots,
                double[] volatilities,
                double interestRate,
                double[] correlations,
                int timestepNumber,
                double[]past,
                int nbRows,
                double [] currents,
                double t,
                double[] trends)
        {

            double[] deltas = new double[6];
            double[] FXRates = new double[6];
            API.DeltasSingleCurrencyBasketAnyTime(
            maturity,
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
            trends,
            FXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates);

            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, deltas, 0, 6); //< -deltas contient maintenant les deltas
            //price et ics contiennent prix et intervalle de couverture selon le pricer
            //on recupère le delta de la formule fermée qu'on met dans realDelta
            double realDelta = RealDeltaAnyTime(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                t);

            if (Math.Abs((realDelta - deltas[0]) / deltas[0]) > 0.05)
            {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("le delta calculé par le pricer à la date rentrée n'est pas bon !");
            }
        }
        public static void PerformDeltaTests0()
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
        public static void PerformDeltaTestsAnyTime()
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
            int timestepNumber = 1;
            double[] trends = new double[1] { 0.05 };
            double t = 1.0;
            double[] past = new double[1] { 0 };
            int nbRows = 1;
            double[] currents = new double[1] { 3.0 };

            DeltaTestAnyTime(maturity,
                optionSize,
                 strike,
                 payoffCoefficients,
                 nbSamples,
                 spots,
                 volatilities,
                 interestRate,
                 correlations,
                 timestepNumber,
                 past,
                 nbRows,
                 currents,
                 t,
                 trends);
        }
    }
}
