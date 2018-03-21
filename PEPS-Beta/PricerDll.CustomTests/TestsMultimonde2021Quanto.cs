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

            Console.WriteLine("Test du Multimonde 2021 quanto sur monde gelé :");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                80.0, 90.0, 100.0, 110.0, 120.0, 130.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
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
            Console.WriteLine("Spots désormais non égaux à 100 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                80.0, 90.0, 100.0, 110.0, 120.0, 130.0,
                1.5, 1.3, 1.1, 0.9, 0.7 };
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
            Console.WriteLine("Taux de change désormais non égaux à 1 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 100");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            past = currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            interestRates = new double[6] { 0.01, 0.01, 0.01, 0.01, 0.01, 0.01 };
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
            Console.WriteLine("Retour au monde gelé basique ; taux d'intérêts tous égaux à 0.01 : ");
            Console.WriteLine("Devrait renvoyer avec certitude 114,60599991");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();
            #endregion
            #region Quasi-déterminés
            nbSamples = 100000;
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

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~174 avec quasi-certitude.");
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

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~115 avec quasi-certitude.");
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

            Console.WriteLine("Test du Multimonde 2021 quanto avec histoire fournie :");
            Console.WriteLine("Devrait renvoyer ~190 avec quasi-certitude.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            #endregion
            #region Incertains
            nbSamples = 100000;
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
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 0,02.");
            Console.WriteLine("Devrait renvoyer légèrement plus que 100 (l'exponentielle est convexe)");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            volatilities = new double[11] {
                0.04, 0.04, 0.04, 0.04, 0.04, 0.04,
                0, 0, 0, 0, 0
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
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 0,04.");
            Console.WriteLine("Devrait renvoyer plus que précedemment.");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
            Console.WriteLine();

            volatilities = new double[11] {
                1, 1, 1, 1, 1, 1,
                0, 0, 0, 0, 0
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
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 1 (absurdement grandes).");
            Console.WriteLine("Devrait renvoyer plus que 100, et de beaucoup (consommation des actifs limités positivement prioritaire)");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);
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
            Console.WriteLine("Doit renvoyer avec certitude 1 pour les actifs et 0 pour les taux de change");
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
        }
    }
}
