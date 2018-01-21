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
        [DllImport(@"C:\Users\Julien\Desktop\PEPS-2017-2018\PEPS-Beta\x64\Debug\PricerDll.dll")]
        extern static double PriceBasket(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double correlation,
            int timestepNumber,
            double[] trends);

        [DllImport(@"C:\Users\Julien\Desktop\PEPS-2017-2018\PEPS-Beta\x64\Debug\PricerDll.dll")]
        extern static double PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double correlation,
            double[] trends
        );

        static void Main(string[] args)
        {
            int optionSize = 40;
            double[] payoffCoefficients = new double[optionSize];
            double[] spots = new double[optionSize];
            double[] volatilities = new double[optionSize];
            double[] trends = new double[optionSize];
            for (int i=0; i<optionSize; i++)
            {
                payoffCoefficients[i] = 0.025;
                spots[i] = 100;
                volatilities[i] = 0.2;
                trends[i] = 0.0;
            }

            /*
            double d = PriceBasket (
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
                0.0,
                trends);

            Console.WriteLine(d);
        }
    }
}