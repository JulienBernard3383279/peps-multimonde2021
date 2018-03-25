using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PEPS_Beta.Models;
using PricerDll.CustomTests;

namespace PEPS_Beta.Tests
{
    [TestClass]
    public class TestPriceCall0
    {
        [TestMethod]
        public unsafe void PriceCall()
        {

            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] spots = new double[1] { 100.0 };
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };
            double price;
            double ic;

            API.PriceBasket(
                maturity, //maturity in years
                optionSize, //optionSize
                strike, //strike when applicable
                payoffCoefficients, //payoffCoefficients
                nbSamples, //nbSamples
                spots, //spots
                volatilities, //volatilities
                interestRate, //interest rate
                correlations, //correlations
                trends, //trends (donc égaux au taux d'intérêt)
                &price,
                &ic);

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = TestsCall.RealPrice(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                0.0);
            // directive assert : Assure que le prix renvoyé par la formule fermée et celui renvoyé par la simulation sont dans un intervalle de confiance de largeur 2%.
            Assert.IsTrue(Math.Abs((realPrice - price) / price) < 0.02);
        }
        }
    }
