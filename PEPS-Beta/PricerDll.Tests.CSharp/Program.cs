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
            while (true)
            {
                int optionSize = 6;
                double[] spots = new double[optionSize];
                double[] volatilities = new double[optionSize];
                double[] trends = new double[optionSize];
                double[] FXRates = new double[optionSize];
                double interestRate;

                Console.WriteLine("Interest rate :");
                interestRate = double.Parse(Console.ReadLine().Replace('.', ','));

                Console.WriteLine("Spots :");
                spots = Console.ReadLine()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("Volatilities :");
                volatilities = Console.ReadLine()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("Trends :");
                trends = Console.ReadLine()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("FX Rates :");
                FXRates = Console.ReadLine()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();
                
                /*for (int i=0; i<optionSize; i++)
                {
                    spots[i] = 100;
                    volatilities[i] = 0.1;
                    trends[i] = 0.0;
                    FXRates[i] = i==0 ? 1.0 : 0.5 ;
                }*/

                double[] correlations = new double[optionSize * optionSize];

                for (int i=0; i<optionSize; i++)
                {
                    for (int j=0; j<optionSize; j++)
                    {
                        correlations[i * optionSize + j] = (i == j) ? 1 : 0;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Lancement de la simulation ...");
                Console.WriteLine();

                double price;
                double ic;

                PriceMultimonde2021(
                    200000,
                    spots,
                    volatilities,
                    interestRate,
                    correlations,
                    trends,
                    &price,
                    &ic);

                double[] deltas = new double[6];
                IntPtr deltasPtr;
                DeltasMultiCurrencyMultimonde2021(
                    200000,
                    spots,
                    volatilities,
                    interestRate,
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
                    200000,
                    spots,
                    volatilities,
                    interestRate,
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
}