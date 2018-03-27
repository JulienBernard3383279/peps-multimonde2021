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
    public class TestPriceCallAnyTime
    {
        [TestMethod] 
        public unsafe void PriceTestCallAnyTime()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] past = new double[1] { 100.0 };
            int nbRows = 1;
            double[] currents = new double[1] { 105.0 };
            double t = 1.0;
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };

            double price;
            double ic;

            API.PriceBasketAnyTime(
                maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                past,
                nbRows,
                t,
                currents,
                volatilities,
                interestRate,
                correlations,
                trends,
                &price,
                &ic);

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = TestsCall.RealPrice(maturity,
                strike,
                currents,
                volatilities,
                interestRate,
                correlations,
                t);
            //assure que prix formule fermée et prix simulé en tout t<T  dans un intervalle de confiance de largeur 2%
            Assert.IsTrue(Math.Abs((realPrice - price) / price) < 0.05);
        }
    }
}
