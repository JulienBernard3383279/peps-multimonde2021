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
    class TestDeltaQuanto0
    {
        [TestMethod]
        public static void DeltaTest0()
        {
            double maturity = 3.0;
            double strike = 100.0;
            int nbSamples = 10000;
            double currFXRate = 1.2;
            double[] interestRates = new double[2] { 0.05, 0.03 }; ;
            double[] spots = new double[2] { 100.0, currFXRate * Math.Exp(-interestRates[1] * maturity) };
            double[] volatilities = new double[2] { 0.01, 0.02 };
            double[] correlations = new double[4] { 1.0, 0.05, 0.05, 1.0 };

            //call quanto = une seule monnaie pour l'actif (un actif quoi), elle est etrangère
            API.SimulDeltasQuanto(
                maturity,
                strike,
                nbSamples,
                spots,
                volatilities,
                interestRates,
                correlations,
                0,
                spots,
                out IntPtr deltasAssets,
                out IntPtr deltasFXRates);

            double date = 0.0;
            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double[] realDelta =TestsQuanto.RealDeltaQuanto0(maturity,
                strike,
                spots,
                volatilities,
                interestRates,
                correlations,
                new double[1] { currFXRate },
                date);

            double[] deltas = new double[2];
            double[] tmp = new double[1];

            System.Runtime.InteropServices.Marshal.Copy(deltasAssets, tmp, 0, 1);
            deltas[0] = tmp[0];
            System.Runtime.InteropServices.Marshal.Copy(deltasFXRates, tmp, 0, 1);
            deltas[1] = tmp[0];

            double realPrice = TestsQuanto.RealPriceQuanto(maturity, strike, spots, volatilities, interestRates, correlations, 0);
            double tmpD = realPrice - realDelta[0] * spots[0] * currFXRate - realDelta[1] * Math.Exp(-interestRates[0] * maturity);
            tmpD /= spots[1];
            realDelta[1] = tmpD;

             Assert.IsTrue(Math.Abs((realDelta[0] - deltas[0]) / deltas[0]) < 0.05);
        }
        }
    }
