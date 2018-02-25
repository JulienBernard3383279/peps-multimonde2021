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
        static Boolean IsNegative(double[] tab)
        {
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] < 0)
                {
                    Console.WriteLine("Don't input negative numbers");
                    return true;
                }
            }
            return false;
        }

        static unsafe void Main(string[] args)
        {
            Console.WriteLine("Enter \"y\" if you want to track the error between our hedging and a simulated path");
            string doYouWantToTrack = Console.ReadLine();
            int thingsToSimulate = (doYouWantToTrack == "y") ? 11 : 6;
            Boolean goToTrackingError = (thingsToSimulate == 11) ? true : false;
            int optionSize = 6;

            while (true)
            {
                int nbSamples = 0;
                do
                {
                    try
                    {
                        Console.WriteLine("Samples number (empty <-> "+(goToTrackingError ? "2 000" : "200 000")+") :");
                        string intermediateNbSamples = Console.ReadLine().Replace('.', ',');
                        nbSamples = (intermediateNbSamples == "") ? (goToTrackingError ? 2000 : 200000)  : int.Parse(intermediateNbSamples);
                        if (nbSamples <= 0)
                        {
                            nbSamples = 0;
                            Console.WriteLine("Please input a positive number");
                        }
                    }
                    catch (FormatException formatException)
                    {
                        Console.WriteLine(formatException.Message);
                        nbSamples = 0;
                    }
                }
                while (nbSamples == 0);

                double t = 0.0;
                Boolean tInvalide = true;
                if (!goToTrackingError)
                {
                    do
                    {
                        try
                        {
                            Console.WriteLine("Current time in year since option creation (in [0 , 6.094], empty <-> 0) :");
                            string intermediateT = Console.ReadLine().Replace('.', ',');
                            t = (intermediateT == "") ? 0 : double.Parse(intermediateT);
                            if (t >= 0 && t <= 6.094)
                            {
                                tInvalide = false;
                            }
                            else
                            {
                                Console.WriteLine("Please input t between 0 and 6.094");
                                tInvalide = true;
                            }
                        }
                        catch (FormatException formatException)
                        {
                            Console.WriteLine(formatException.Message);
                            tInvalide = true;
                        }
                    }
                    while (tInvalide);
                }

                double[] spotsOrCurrent = new double[optionSize];
                double[] volatilities = new double[thingsToSimulate];
                double[] trends = new double[optionSize];
                double[] FXRates = new double[optionSize];
                double interestRate = 0.0;

                Boolean spotsInvalide = true;
                do
                {
                    try
                    {
                        Console.WriteLine((t == 0.0 ? "Spots" : "Current prices") + " (empty <-> 6 times 100) :");
                        string intermediateSpotsOrCurrent = Console.ReadLine();
                        spotsOrCurrent = (intermediateSpotsOrCurrent == "") ? new double[6] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 } :
                            intermediateSpotsOrCurrent
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => double.Parse(s.Replace('.', ',')))
                            .ToArray();
                        spotsInvalide = IsNegative(spotsOrCurrent);

                    }
                    catch (FormatException formatException)
                    {
                        Console.WriteLine(formatException.Message);
                        spotsInvalide = true;
                    }
                }
                while (spotsInvalide || spotsOrCurrent.Length != optionSize);


                Boolean volatilitiesInvalide = true;
                do
                {
                    try
                    {
                        Console.WriteLine("Volatilities  (empty <-> " + thingsToSimulate + " times 0.08) :");
                        string intermediateVolatilities = Console.ReadLine();
                        double[] tmpTab = (thingsToSimulate == 11) ? new double[11] { 0.08, 0.08, 0.08, 0.08, 0.08, 0.08, 0.08, 0.08, 0.08, 0.08, 0.08 } : new double[6] { 0.08, 0.08, 0.08, 0.08, 0.08, 0.08 };
                        volatilities = (intermediateVolatilities == "") ? tmpTab :
                            intermediateVolatilities
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => double.Parse(s.Replace('.', ',')))
                            .ToArray();
                        volatilitiesInvalide = IsNegative(volatilities);
                    }
                    catch (FormatException formatException)
                    {
                        Console.WriteLine(formatException.Message);
                        volatilitiesInvalide = true;
                    }
                }
                while (volatilitiesInvalide || volatilities.Length != thingsToSimulate);

                Boolean rfInvalide = true;
                do
                {
                    try
                    {
                        Console.WriteLine("Risk-free interest rate (empty <-> 6 times 0) :");
                        string intermediateTrends = Console.ReadLine();
                        trends = (intermediateTrends == "") ? new double[6] { 0, 0, 0, 0, 0, 0 } :
                            intermediateTrends
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => double.Parse(s.Replace('.', ',')))
                            .ToArray();
                        interestRate = trends[0];
                        rfInvalide = IsNegative(trends);
                    }
                    catch (FormatException formatException)
                    {
                        Console.WriteLine(formatException.Message);
                        rfInvalide = true;
                    }
                }
                while (rfInvalide || trends.Length != optionSize);



                Boolean FXInvalide = true;
                do
                {
                    try
                    {
                        Console.WriteLine("FX Rates (empty <-> 5 times 1) :");
                        string intermediateFXRates = Console.ReadLine();
                        FXRates = (intermediateFXRates == "") ? new double[6] { 1, 1, 1, 1, 1, 1 } :
                            ("1 " + intermediateFXRates)
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => double.Parse(s.Replace('.', ',')))
                            .ToArray();
                        if (FXRates.Length == optionSize - 1)
                        {
                            double[] tmptab = new double[6];
                            tmptab[0] = 1;
                            for (int i = 0; i < 5; i++)
                            {
                                tmptab[i + 1] = FXRates[i];
                            }
                            FXRates = tmptab;
                        }
                        if (FXRates.Length == optionSize && FXRates[0] != 1)
                        {
                            Console.WriteLine("Please input 5 rates, or 6 if the first one is equal to 1");
                            FXInvalide = true;
                        }
                        else
                        {
                            FXInvalide = IsNegative(FXRates);
                        }
                    }
                    catch (FormatException formatException)
                    {
                        Console.WriteLine(formatException.Message);
                        FXInvalide = true;
                    }
                }
                while (FXInvalide);

                Console.WriteLine("Correlations cannot be entered yet. Currently, all assets are assumed independant.");

                double[] correlations = new double[thingsToSimulate * thingsToSimulate];

                for (int i = 0; i < thingsToSimulate; i++)
                {
                    for (int j = 0; j < thingsToSimulate; j++)
                    {
                        correlations[i * thingsToSimulate + j] = (i == j) ? 1 : 0;
                    }
                }

                double[] past = null;
                int nbRows = 0;

                if (t != 0)
                {
                    nbRows = 1 + (int)(t / (371.0 / 365.25));

                    Boolean pastInvalide = true;
                    do
                    {
                        string intermediatePast = "";
                        Console.WriteLine("Past (cannot leave empty) :");
                        Console.WriteLine("(It is the prices of the assets on passed constatation dates)");
                        try
                        {
                            for (int i = 0; i < nbRows; i++)
                            {
                                intermediatePast += Console.ReadLine() + " ";
                            }
                            past = intermediatePast
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => double.Parse(s.Replace('.', ',')))
                                .ToArray();
                            pastInvalide = IsNegative(past);
                            pastInvalide = pastInvalide || (past.Length != nbRows * optionSize);
                        }
                        catch (FormatException formatException)
                        {
                            Console.WriteLine(formatException.Message);
                            pastInvalide = true;
                        }

                    }
                    while (pastInvalide);
                }

                Console.WriteLine();
                Console.WriteLine("Simulation started ...");
                Console.WriteLine();

                double price;
                double ic;

                if (goToTrackingError)
                {

                    double tracking_error;

                    API.TrackingErrorMultimonde(
                        nbSamples,
                        spotsOrCurrent,
                        volatilities,
                        interestRate,
                        correlations,
                        FXRates,
                        trends,
                        &tracking_error);

                    Console.WriteLine();
                    Console.WriteLine("Tracking Error associée aux paramètres rentrés :" + tracking_error);
                    Console.WriteLine();
                    Console.WriteLine("===== New entry =====");
                    Console.WriteLine();
                }
                else
                {
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
}