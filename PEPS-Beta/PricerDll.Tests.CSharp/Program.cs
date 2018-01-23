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
        unsafe extern static double PriceBasket(
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
            double[] trends);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        unsafe extern static double PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double[] trends
        );

        static void Main(string[] args)
        {
            int optionSize = 6;
            double[] spots = new double[optionSize];
            double[] volatilities = new double[optionSize];
            double[] trends = new double[optionSize];
            for (int i=0; i<optionSize; i++)
            {
                spots[i] = 100;
                volatilities[i] = 0.2;
                trends[i] = 0.0;
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

            double d = PriceMultimonde2021(
                100000,
                spots,
                volatilities,
                0.0,
                correlations,
                trends);


            Console.WriteLine(d);
        }
    }
}