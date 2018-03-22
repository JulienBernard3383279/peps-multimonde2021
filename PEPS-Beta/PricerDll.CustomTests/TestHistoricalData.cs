using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestHistoricalData
    {
        public static void TestHisto()
        {
            DataStorage ds = new DataStorage();
            ds.FillData();
            ds.DataToArray();
            double[,] data_ = ds.IndexValues;

            DataCurrencies dc = new DataCurrencies();
            dc.Fill();
            dc.DataToArray();
            double[,] dataFX = dc.ChangeValues;

            Console.WriteLine("les valeurs ont bien été obtenues du web !");
            int nbSamples =  200000;

            Console.WriteLine("Current time in year since option creation (in [0 , 6.094], empty <-> 0) :");
            string intermediateT = Console.ReadLine().Replace('.', ',');
            double t = (intermediateT == "") ? 0 : double.Parse(intermediateT);
            

           int nb = 1 + (int)(t / (371.0 / 365.25));

            int optionSize = 6;
            double[] spotsOrCurrent = new double[optionSize];
            double[] volatilities = new double[optionSize];
            double[] trends = new double[optionSize];
            double[] FXRates = new double[optionSize];
            double interestRate;

            
            interestRate = 0.021;

            if (t == 0)
            {
                spotsOrCurrent = new double[6] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 };
            }
            else
            {
                spotsOrCurrent = new double[6] { data_[0, nb], data_[1, nb], data_[2, nb], data_[3, nb], data_[4, nb], data_[5, nb] };

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine(spotsOrCurrent[i]);
                }
            }

            double[,] covMat = PricerDll.CustomTests.MathUtils.ComputeCovMatrix(PricerDll.CustomTests.MathUtils.ComputeReturns(data_));
            volatilities = PricerDll.CustomTests.MathUtils.ComputeVolatility(covMat);

            trends = new double[6] {  0.021, 0.031, 0.024, 0.021, 0.007, 0.045};
            //trends = new double[6] { 0,0,0,0,0,0};


            FXRates = new double[6] { 1,dataFX[0,248+nb], dataFX[1, 248 + nb], dataFX[2, 248 + nb], dataFX[3, 248 + nb], dataFX[4, 248 + nb] };
            //FXRates = new double[6] { 1, 1, 1, 1, 1, 1 };

            double[] correlations = new double[optionSize * optionSize];

            double[,] corrMat = PricerDll.CustomTests.MathUtils.ComputeCorrMatrix(covMat);
            Console.WriteLine("les correlations valent :");
            for (int i = 0; i < optionSize; i++)
            {
                for (int j = 0; j < optionSize; j++)
                {
                    correlations[i * optionSize + j] = corrMat[i, j];
                    Console.WriteLine(correlations[i * optionSize + j]);
                }
            }

           
            double[] past = null;
            int nbRows = 0;

            if (t != 0)
            {
                nbRows = 1 + (int)(t / (371.0 / 365.25));
                //string intermediatePast = "";
                //Console.WriteLine("Past (cannot leave empty) :");
                past = new double[5 * nbRows];
                for (int j = 0; j < nbRows; j++)
                {
                    //intermediatePast += Console.ReadLine() + " ";
                    for (int i = 0;i < 5; i++)
                    {
                        past[i * nbRows + j] = data_[i, j];
                    }
                }
                /*past = intermediatePast
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Replace('.', ',')))
                    .ToArray();*/
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
                System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 6);
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
                System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 6);
            }

            //Marshal.FreeCoTaskMem(deltasPtr); "PricerDll.Tests.CSharp a cessé de fonctionner." Ah.

            double[] deltasAssets = new double[6];
            double[] deltasFXRates = new double[6];

            if (t == 0)
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

                System.Runtime.InteropServices.Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
                System.Runtime.InteropServices.Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);
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

                System.Runtime.InteropServices.Marshal.Copy(deltasAssetsPtr, deltasAssets, 0, 6);
                System.Runtime.InteropServices.Marshal.Copy(deltasFXRatesPtr, deltasFXRates, 0, 6);
            }


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
            Console.WriteLine("Quantité de monnaie à acheter : ");
            for (int i = 1; i < 6; i++)
            {
                Console.WriteLine(deltasFXRates[i]);
            }
            Console.WriteLine();




        }
    }
}
