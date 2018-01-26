using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PricerConsole
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Samples number (empty <-> 200 000) :");
                string intermediateNbSamples = Console.ReadLine().Replace('.', ',');
                int nbSamples = (intermediateNbSamples == "") ? 200000 : int.Parse(intermediateNbSamples);

                Console.WriteLine("Current time in year since option creation (in [0 , 6.094], empty <-> 0) :");
                string intermediateT = Console.ReadLine().Replace('.', ',');
                double t = (intermediateT == "") ? 0 : double.Parse(intermediateT);

                int optionSize = 6;
                double[] spotsOrCurrent = new double[optionSize];
                double[] volatilities = new double[optionSize];
                double[] trends = new double[optionSize];
                double[] FXRates = new double[optionSize];
                double interestRate;

                Console.WriteLine("Interest rate (empty <-> 0) :");
                string intermediateInterestRate = Console.ReadLine().Replace('.', ',');
                interestRate = (intermediateInterestRate == "") ? 0 : double.Parse(intermediateInterestRate);

                Console.WriteLine( (t == 0.0 ? "Spots" : "Current prices")+" (empty <-> 6 times 100) :");
                string intermediateSpotsOrCurrent = Console.ReadLine();
                spotsOrCurrent = (intermediateSpotsOrCurrent=="") ? new double[6] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 } :
                    intermediateSpotsOrCurrent
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("Volatilities  (empty <-> 6 times 0.08) :");
                string intermediateVolatilities = Console.ReadLine();
                volatilities = (intermediateVolatilities=="") ? new double[6] { 0.08, 0.08, 0.08, 0.08, 0.08, 0.08 } :
                    intermediateVolatilities
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("Trends (empty <-> 6 times 0) :");
                string intermediateTrends = Console.ReadLine();
                trends = (intermediateTrends == "") ? new double[6] { 0, 0, 0, 0, 0, 0 } :
                    intermediateTrends
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                Console.WriteLine("FX Rates (empty <-> 5 times 1) :");
                string intermediateFXRates = Console.ReadLine();
                FXRates = (intermediateFXRates == "") ? new double[6] { 1, 1, 1, 1, 1, 1 } :
                    ("1 "+intermediateFXRates)
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();

                double[] correlations = new double[optionSize * optionSize];

                for (int i=0; i<optionSize; i++)
                {
                    for (int j=0; j<optionSize; j++)
                    {
                        correlations[i * optionSize + j] = (i == j) ? 1 : 0;
                    }
                }

                double[] past = null;
                int nbRows = 0;

                if (t != 0)
                {
                    nbRows = 1 + (int)(t / (371.0 / 365.25));
                    string intermediatePast = "";
                    Console.WriteLine("Past (cannot leave empty) :");
                    for (int i = 0; i < nbRows; i++)
                    {
                        intermediatePast += Console.ReadLine() + " ";
                    }
                    past = intermediatePast
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => double.Parse(s.Replace('.', ',')))
                        .ToArray();
                }
                
                Console.WriteLine();
                Console.WriteLine("Lancement de la simulation ...");
                Console.WriteLine();

                double price;
                double ic;

                /*

                double tracking_error;

                API.TrackingErrorMultimonde(
                    1000,
                    spotsOrCurrent,
                    volatilities,
                    interestRate,
                    correlations,
                    FXRates,
                    trends,
                    &tracking_error);

                Console.WriteLine();
                Console.WriteLine("Tracking Error associée aux paramètres rentrés :" + tracking_error);
                */
                
                if (t == 0)
                {
                    API.PriceMultimonde2021(
                        nbSamples,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        &price,
                        &ic);
                }
                else
                {
                    API.PriceMultimonde2021AnyTime(
                        nbSamples,
                        past,
                        nbRows,
                        t,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        &price,
                        &ic);
                }
                
                
                double[] deltas = new double[6];

                if (t == 0)
                {
                    API.DeltasMultiCurrencyMultimonde2021(
                        nbSamples,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        out IntPtr deltasPtr);
                    Marshal.Copy(deltasPtr, deltas, 0, 6);
                }
                else
                {
                    API.DeltasMultiCurrencyMultimonde2021AnyTime(
                        nbSamples,
                        past,
                        nbRows,
                        t,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        out IntPtr deltasPtr);
                    Marshal.Copy(deltasPtr, deltas, 0, 6);

                }

                //Marshal.FreeCoTaskMem(deltasPtr); "PricerDll.Tests.CSharp a cessé de fonctionner." Ah.

                double[] deltasAssets = new double[6];
                double[] deltasFXRates = new double[6];

                API.ConvertDeltas(deltas,
                    spotsOrCurrent,
                    FXRates,
                    out IntPtr deltasAssetsPtr,
                    out IntPtr deltasFXRatesPtr);
                Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
                Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);

                /*if (t == 0)
                {
                    API.DeltasSingleCurrencyMultimonde2021(
                        nbSamples,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        FXRates,
                        out IntPtr deltasAssetsPtr,
                        out IntPtr deltasFXRatesPtr);

                    Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
                    Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);
                }
                else
                {
                    API.DeltasSingleCurrencyMultimonde2021AnyTime(
                        nbSamples,
                        past,
                        nbRows,
                        t,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        trends,
                        FXRates,
                        out IntPtr deltasAssetsPtr,
                        out IntPtr deltasFXRatesPtr);

                    Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
                    Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);
                }*/
                
                Console.WriteLine("Prix Multimonde : " + price);
                Console.WriteLine("Intervalle de confiance Multimonde : " + ic);
                Console.WriteLine();
                Console.WriteLine("Deltas intermédiaires (indicatif) : ");
                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine(deltas[i]);
                }
                Console.WriteLine();
                Console.WriteLine("Nombre d'actifs à acheter : ");
                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine(deltasAssets[i]);
                }
                Console.WriteLine();
                Console.WriteLine("Quantité de monnaie étrangère à acheter : ");
                for (int i = 1; i < 6; i++)
                {
                    Console.WriteLine(deltasFXRates[i]);
                }
                Console.WriteLine();
                Console.WriteLine("Euros restants à mettre au taux sans risque européen : ");
                Console.WriteLine(price - deltasAssets[0] * spotsOrCurrent[0]);
                Console.WriteLine();
                Console.WriteLine("===== New entry =====");
                Console.WriteLine();

            }
        } 
    }
}