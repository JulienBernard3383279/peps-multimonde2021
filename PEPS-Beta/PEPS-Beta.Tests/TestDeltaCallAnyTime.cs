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
    public class TestDeltaCallAnyTime
    {
        [TestMethod]
        public void DeltaTestAnyTime()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 1000000;
            double[] volatilities = new double[1] { 0.05 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            double[] trends = new double[1] { 0.05 };
            double t = 1.0;
            double[] past = new double[1] { 100.00 };
            int nbRows = 1;
            double[] currents = new double[1] { 95.0 };
            double[] deltas = new double[1];

            API.DeltasMultiCurrencyBasketAnyTime(
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
                 out IntPtr deltasPtr);

            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 1); //< -deltas contient maintenant les deltas

            double realDelta = TestsCall.RealDeltaAnyTime(maturity,
                strike,
                currents,
                volatilities,
                interestRate,
                correlations,
                t);
            Assert.IsTrue(Math.Abs((realDelta - deltas[0]) / deltas[0]) < 0.02);

        }
    }
}

