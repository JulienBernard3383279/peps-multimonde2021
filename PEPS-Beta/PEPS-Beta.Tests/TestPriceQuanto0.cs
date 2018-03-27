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
    public class TestPriceQuanto0
    {
        [TestMethod]
        public unsafe void PriceTestQuanto()
        {
            double maturity = 3.0;
            double strike = 100.0;
            int nbSamples = 1000000;
            double currFXRate = 1.0;
            double[] interestRates = new double[2] { 0.05, 0.05 };
            double[] spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * maturity) };
            double[] volatilities = new double[2] { 0.05, 0.00 };
            double[] correlations = new double[4] { 1.0, 0.0, 0.0, 1.0 };

            double price;
            double ic;

            API1.PriceQuanto(
                maturity, //maturity in years
                strike, //strike when applicable
                nbSamples, //nbSamples
                spots, //spots
                volatilities, //volatilities
                interestRates, //interest rate
                correlations, //correlations
                0.0,
                spots,
                &price,
                &ic);

            double date = 0.0;
            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = TestsQuanto.RealPriceCallQuanto(maturity,
                strike,
                spots,
                volatilities,
                interestRates,
                correlations,
                date);
            Assert.IsTrue((realPrice < price + 1.5 * ic / 2) || (realPrice > price - 1.5 * ic / 2));
           
        }

    }
}
