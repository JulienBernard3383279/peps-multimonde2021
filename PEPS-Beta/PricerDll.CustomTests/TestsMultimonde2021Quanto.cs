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
            #region Certains
            int nbSamples = 100000;

            double[] currentPrices = new double[11] {
                100.0, 100.0, 100.0, 100.0, 100.0, 100.0,
                1.0, 1.0, 1.0, 1.0, 1.0 };
            double[] volatilities = new double[11] {
                0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };
            double[] interestRates = new double[6] {
                0, 0, 0, 0, 0, 0
            };
            double[] correlations = new double[11 * 11];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    correlations[11 * i + j] = i == j ? 1 : 0;
                }
            }
            double[] past = currentPrices;
            int nbRows = 1;
            double t = 0;

            double price;
            double ic;

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
            Console.WriteLine("Monde basique ; volatilités des actifs (pas des taux de change) modifiés à 2%.");
            Console.WriteLine("Devrait renvoyer légèrement plus que 100 (l'exponentielle est convexe)");
            Console.WriteLine("Prix : " + price);
            Console.WriteLine("Largeur de l'intervalle de confiance : " + ic);

            Console.WriteLine();
            #endregion
        }
    }
}
