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
    public class TestPriceMultiMonde2021Quanto0
    {
        [TestMethod]
        public unsafe void PerformPriceTests()
        {
           
            int nbSamples;
            double[] currentPrices;
            double[] volatilities;
            double[] interestRates;
            double[] correlations;
            double[] past;
            int nbRows;
            double t;
            double price;
            double ic;
           
            
            nbSamples = 100000;
            currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0
            };
            volatilities = new double[11] {
                0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };
            interestRates = new double[6] {
                0, 0, 0, 0, 0, 0
            };
            correlations = new double[11 * 11];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    correlations[11 * i + j] = i == j ? 1 : 0;
                }
            }
            past = currentPrices;
            nbRows = 1;
            t = 0;

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

            Assert.IsTrue(price == 100);


        }
    }
}
