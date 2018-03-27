using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PricerDll.CustomTests;
//using PricerDll;

namespace PEPS_Beta.Tests
{
    [TestClass]
    public class TestDeltaCall0
    {
        [TestMethod]
        public void TestDeltaCall_0()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] spots = new double[1] { 100.0 };
            double[] volatilities = new double[1] { 0.1 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
           // int timestepNumber = 1;
            double[] trends = new double[1] { 0.05 };

            API1.DeltasMultiCurrencyBasket(
                 maturity,
                 optionSize,
                 strike,
                 payoffCoefficients,
                 nbSamples,
                 spots,
                 volatilities,
                 interestRate,
                 correlations,
                 trends,
                 out IntPtr deltasPtr);

            double[] deltas = new double[1];
            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 1); //< -deltas contient maintenant les deltas

            double realDelta = PricerDll.CustomTests.TestsCall.RealDelta0(maturity,
                strike,
                spots,
                volatilities,
                interestRate,
                correlations,
                0.0);
            //Assure que le delta calculé et simulé sont dans un intervalle de confiance de largeur inferieure à 2%.
            Assert.IsTrue(Math.Abs((realDelta - deltas[0]) / deltas[0]) < 0.02);
        }
    }
}
