﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class TestsCall
    {
        private static double RealPrice(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int nbSamples,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            int timestepNumber,
            double[] trends) //potentiellement pas tous nécessaires
        {
            return 0; //formule fermée du prix
        }

        private static void PriceTest(double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int nbSamples,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            int timestepNumber,
            double[] trends)
        {

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
                timestepNumber,
                trends, //trends (donc égaux au taux d'intérêt)
                &price,
                &ic);

            //price et ics contiennent prix et intervalle de couverture selon le pricer

            double realPrice = RealPrice(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRate,
                correlations,
                timestepNumber,
                trends);

            if (Math.Abs( (realPrice-price)/price) > 0.05) {
                // Le prix trouvé par le pricer est plus de 5% à côté du vrai prix !
                Console.WriteLine("problème !");
            }
        }

        public static void PerformPriceTests()
        {
            double maturity = 3.0;
            int optionSize = 1;
            double strike = 100.0;
            double[] payoffCoefficients = new double[1] { 1.0 };
            int nbSamples = 100000;
            double[] spots = new double[1] { 1.0 };
            double[] volatilities = new double[1] { 1.0 };
            double interestRate = 0.05;
            double[] correlations = new double[1] { 1.0 };
            int timestepNumber = 1;
            double[] trends = new double[1] { 1.0 };

            PriceTest(maturity,
                optionSize,
                strike,
                payoffCoefficients,
                nbSamples,
                spots,
                volatilities,
                interestRate,
                correlations,
                timestepNumber,
                trends);
        }
    }
}