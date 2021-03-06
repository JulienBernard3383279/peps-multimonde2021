﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            #region Init
            Console.WriteLine("Tests prix du multimonde 2021 quanto");
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
            #endregion
            #region Certains
            nbSamples = 100_000;
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

            var watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            var executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Test du Multimonde 2021 quanto sur monde gelé :");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                80.0, 90.0, 100.0, 110.0, 120.0, 130.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Spots désormais non égaux à 100 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                80.0, 90.0, 100.0, 110.0, 120.0, 130.0,
                1.5, 1.3, 1.1, 0.9, 0.7 };
            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Taux de change désormais non égaux à 1 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            interestRates = new double[6] { 0.01, 0.01, 0.01, 0.01, 0.01, 0.01 };
            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Retour au monde gelé basique ; taux d'intérêts tous égaux à 0.01 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 114,60599991");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();
            #endregion
            #region Quasi-déterminés
            nbSamples = 100_000;
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
            nbRows = 6;
            t = (371 / 365.25) * 5.999;
            past = new double[6 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            currentPrices = new double[11] {
                99.0, 100000.0, 100000.0, 100000.0, 100000.0, 100000.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~174 avec quasi-certitude.");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = new double[6 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 102.0, 104.0, 106.0, 108.0, 110.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 102.0, 104.0, 106.0, 108.0, 0,     1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 102.0, 104.0, 106.0, 0,     0,     1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 102.0, 104.0, 0,     0,     0,     1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 102.0, 0,     0,     0,     0,     1.0, 1.0, 1.0, 1.0, 1.0,
            };
            currentPrices = new double[11] {
                50, 0, 0, 0, 0, 0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~115 avec quasi-certitude.");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = new double[6 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                120.0, 100.0, 104.0, 106.0, 108.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 120.0, 100.0, 106.0, 108.0, 0,     1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 100.0, 130.0, 106.0, 110.0, 3,     1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 100.0, 100.0, 150.0, Math.PI, 0,   1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 100.0, 100.0, 100.0, 200.0, 0,     1.0, 1.0, 1.0, 1.0, 1.0,
            };
            currentPrices = new double[11] {
                -1/12, 0, 0, 0, 0, Math.E*10000, 1.0, 1.0, 1.0, 1.0, 1.0
            }; // la fatigue

            watch = System.Diagnostics.Stopwatch.StartNew();
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
            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~190 avec quasi-certitude.");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            #endregion
            #region Incertains
            nbSamples = 100_000;
            currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            volatilities = new double[11] {
                0.02, 0.02, 0.02, 0.02, 0.02, 0.02,
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

            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 0,02.");
            Console.WriteLine("Devrait renvoyer légèrement plus que 100 (l'exponentielle est convexe)");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            volatilities = new double[11] {
                0.04, 0.04, 0.04, 0.04, 0.04, 0.04,
                0, 0, 0, 0, 0
            };
            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 0,04.");
            Console.WriteLine("Devrait renvoyer plus que précedemment.");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            volatilities = new double[11] {
                0.1, 0.1, 0.1, 0.1, 0.1, 0.1,
                0, 0, 0, 0, 0
            };
            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 0,1.");
            Console.WriteLine("Devrait renvoyer plus que 100");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            #endregion
            #region Incertains semi-déterminés
            nbSamples = 100_000;
            currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            volatilities = new double[11] {
                0.02, 0.02, 0.02, 0.02, 0.02, 0.02,
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
            nbRows = 3;
            t = (371 / 365.25) * 2.999;
            past = new double[3 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                110.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 110.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            currentPrices = new double[11] {
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("t=2.999 nbRows=3");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            nbRows = 4;
            t = (371 / 365.25) * 3.000;
            past = new double[4 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                110.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 110.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            currentPrices = new double[11] {
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("t=3.000 nbRows=4");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            nbRows = 4;
            t = (371 / 365.25) * 3.001;
            past = new double[4 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                110.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 110.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            currentPrices = new double[11] {
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            watch = System.Diagnostics.Stopwatch.StartNew();
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

            watch.Stop();
            executionTime = watch.ElapsedMilliseconds;

            Console.WriteLine("t=3.001 nbRows=4");
            Console.WriteLine("Calcul en " + executionTime + " millisecondes.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            #endregion
        }

        public static unsafe void PerformDeltaTest()
        {
            #region Init
            Console.WriteLine("Tests deltas du multimonde 2021 quanto");
            int nbSamples;
            double[] currentPrices;
            double[] volatilities;
            double[] interestRates;
            double[] correlations;
            double[] past;
            int nbRows;
            double t;
            IntPtr deltasPtr;
            #endregion
            #region Certains
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

            API.DeltasMultimonde2021Quanto(
                nbSamples,
                past,
                nbRows,
                t,
                currentPrices,
                volatilities,
                interestRates,
                correlations,
                out deltasPtr);

            double[] deltas = new double[11];
            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 11); //<- deltas contient maintenant les deltas

            Console.WriteLine("Calcul des deltas sur monde gelé basique.");
            Console.WriteLine("Doit renvoyer avec certitude 1 pour les actifs et -100 pour les taux de change");
            for (int i = 0; i < 11; i++) Console.WriteLine(String.Format("{0:######0.######}", deltas[i]) + " ");
            Console.WriteLine();


            interestRates = new double[6] {
                0.01, 0.01, 0.01, 0.01, 0.01, 0.01
            };
            API.DeltasMultimonde2021Quanto(
                nbSamples,
                past,
                nbRows,
                t,
                currentPrices,
                volatilities,
                interestRates,
                correlations,
                out deltasPtr);

            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 11); //<- deltas contient maintenant les deltas

            Console.WriteLine("Calcul des deltas sur monde gelé avec taux d'intérêt = 1%.");
            for (int i = 0; i < 11; i++) Console.WriteLine(String.Format("{0:######0.######}", deltas[i]) + " ");
            Console.WriteLine();
            #endregion
            #region Incertains semi-déterminés
            nbSamples = 100_000;
            currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            volatilities = new double[11] {
                0.02, 0.02, 0.02, 0.02, 0.02, 0.02,
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
            nbRows = 4;
            t = (371 / 365.25) * 3;
            past = new double[4 * 11]
            {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                110.0, 100.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                100.0, 110.0, 100.0, 100.0, 100.0, 100.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };
            currentPrices = new double[11] {
                10000, 10000, 105.0, 102.0, 98.0, 90.0, 1.0, 1.0, 1.0, 1.0, 1.0
            };

            var watch = System.Diagnostics.Stopwatch.StartNew();
            API.DeltasMultimonde2021Quanto(
                nbSamples,
                past,
                nbRows,
                t,
                currentPrices,
                volatilities,
                interestRates,
                correlations,
                out deltasPtr);

            watch.Stop();
            var executionTime = watch.ElapsedMilliseconds;

            deltas = new double[11];
            System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 11); //<- deltas contient maintenant les deltas

            Console.WriteLine("Calcul des deltas sur monde gelé basique.");
            Console.WriteLine("Devrait renvoyer 0 pour l'actif sélectionné à cette date de constatation (le n°3 <-> index 2).");
            for (int i = 0; i < 11; i++) Console.WriteLine(String.Format("{0:######0.######}", deltas[i]) + " ");
            Console.WriteLine();
            #endregion
        }

        public static unsafe void PerformTrackingErrorTest()
        {
            #region Init
            Console.WriteLine("Tests de la tracking error du Multimonde 2021 Quanto");
            int nbSamples;
            double[] currentPrices;
            double[] volatilities;
            double[] interestRates;
            double[] correlations;
            double[] past;
            int nbRows;
            double t;
            double trackingError;
            double relativePricingVolatility;
            int nbUpdates;
            #endregion
            #region Test
            nbSamples = 5_000;
            interestRates = new double[6] {
                0.02,0.02,0.02,0.02,0.02,0.02
            };
            currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0,
                1.0,
                1.0,
                1.0,
                1.0
            };
            volatilities = new double[11] {
                0.02, 0.02, 0.02, 0.02, 0.02, 0.02,
                0.02, 0.02, 0.02, 0.02, 0.02
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
            nbUpdates = 52*6;

            API.TrackingErrorMultimonde2021Quanto(
                nbSamples,
                0 * (371.0 / 365.25),
                6 * (371.0 / 365.25),
                t,
                past,
                nbRows,
                currentPrices, //spots
                currentPrices, //prices at simulation start
                volatilities,
                interestRates,
                correlations,
                nbUpdates,
                &trackingError,
                &relativePricingVolatility,
                out IntPtr portfolioReturnsPtr,
                out IntPtr productReturnsPtr);

            double[] portfolioReturns = new double[nbUpdates];
            System.Runtime.InteropServices.Marshal.Copy(portfolioReturnsPtr, portfolioReturns, 0, nbUpdates);

            double[] productReturns = new double[nbUpdates];
            System.Runtime.InteropServices.Marshal.Copy(productReturnsPtr, productReturns, 0, nbUpdates); 

            Console.WriteLine("Mensual tracking error : " + trackingError);
            Console.WriteLine("Mensual impact of the relative pricing volatility : " + relativePricingVolatility);


            #endregion
        }
    }
}
