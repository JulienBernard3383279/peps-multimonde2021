using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    class TestsMultimonde2021Quanto
    {
        public static unsafe void PerformPriceTests()
        {
            int nbSamples = 100000;

            double[] currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            double[] volatilities = new double[11] {
                0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };
            double[] interestRates = new double[6] {
                0, 0, 0, 0, 0, 0
            };
            double[] correlations = new double[11 * 11];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    correlations[11 * i + j] = i == j ? 1 : 0;
                }
            }
            double[] past = currentPrices;
            int nbRows = 1;
            double t = 0;

            double price;
            double ic;

            API.PriceMultimonde2021Quanto(
                nbSamples,
                past,
                nbRows,
                t,
                currentPrices,
                volatilities,
                interestRates,
                correlations,
                &price,
                &ic);

            Console.WriteLine("Test du Multimonde 2021 quanto sur monde gelé :");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
        }
    }
}
