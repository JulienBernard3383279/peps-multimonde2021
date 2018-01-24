using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.Tests.CSharp
{
    class Program
    {
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        unsafe extern static void PriceBasket(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            int timestepNumber,
            double[] trends,
            double* price,
            double* ic);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        unsafe extern static void PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double[] trends,
            double* price,
            double* ic
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        unsafe extern static void DeltasMultiCurrencyMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            out IntPtr deltas
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        unsafe extern static void DeltasSingleCurrencyMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends,
            double[] FXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );

        static unsafe void Main(string[] args)
        {
            int optionSize = 6;
            double[] spots = new double[optionSize];
            double[] volatilities = new double[optionSize];
            double[] trends = new double[optionSize];
            double[] FXRates = new double[optionSize];

            for (int i=0; i<optionSize; i++)
            {
                spots[i] = 100;
                volatilities[i] = 0.1;
                trends[i] = 0.0;
                FXRates[i] = i==0 ? 1.0 : 0.5 ;
            }

            double[] correlations = new double[optionSize * optionSize];
            for (int i=0; i<optionSize; i++)
            {
                for (int j=0; j<optionSize; j++)
                {
                    correlations[i * optionSize + j] = (i == j) ? 1 : 0;
                }
            }

            /*trends[0] = -0.5;
            trends[1] = -0.5;
            trends[2] = 0.5;
            trends[3] = 0.5;
            trends[4] = 0.5;
            trends[5] = 0.5;*/

            /*double d = PriceBasket (
                3.0, //maturity in years
                40, //optionSize
                100, //strike when applicable
                payoffCoefficients, //payoffCoefficients
                50000, //nbSamples
                spots, //spots
                volatilities, //volatilities
                0.04879, //interest rate
                0.0, //correlation
                1, //osef if 1
                trends ); //trends
                */

            double price;
            double ic;

            PriceMultimonde2021(
                100000,
                spots,
                volatilities,
                0.0,
                correlations,
                trends,
                &price,
                &ic);

            double[] deltas = new double[6];
            IntPtr deltasPtr;
            DeltasMultiCurrencyMultimonde2021(
                1000000,
                spots,
                volatilities,
                0.0,
                correlations,
                trends,
                out deltasPtr);
            Marshal.Copy(deltasPtr, deltas, 0, 6);
            //Marshal.FreeCoTaskMem(deltasPtr); "PricerDll.Tests.CSharp a cessé de fonctionner." Ah.

            double[] deltasAssets = new double[6];
            IntPtr deltasAssetsPtr;
            double[] deltasFXRates = new double[6];
            IntPtr deltasFXRatesPtr;

            DeltasSingleCurrencyMultimonde2021(
                1000000,
                spots,
                volatilities,
                0.0,
                correlations,
                trends,
                FXRates,
                out deltasAssetsPtr,
                out deltasFXRatesPtr);

            Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
            Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);


            Console.WriteLine("Prix Multimonde : " + price);
            Console.WriteLine("Intervalle de confiance Multimonde : " + ic);
            Console.WriteLine();
            Console.WriteLine("Deltas Multimonde en monnaies étrangères : ");
            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine(deltas[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Deltas Multimonde en Euros - actifs : ");
            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine(deltasAssets[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Deltas Multimonde en Euros - taux de change : ");
            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine(deltasFXRates[i]);
            }
            Console.WriteLine();
        }
    }
}