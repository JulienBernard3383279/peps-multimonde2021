using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PricerDll.CustomTests;

namespace PEPS_Beta.Tests
{
    [TestClass]
    public class TestPriceMultiMonde2021QuantoAnyTime
    {
        [TestMethod]
        public unsafe void PerformPriceTestAnyTime()
        {
            double price;
            double ic;
            int nbSamples = 100000;
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
            int nbRows = 6;
            double t = (371 / 365.25) * 5.999;
            double[] past = new double[6 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            double[] currentPrices = new double[11] {
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

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


        }
    }
}
