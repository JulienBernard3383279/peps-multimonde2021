﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PEPS_Beta.Tests
{
    public static unsafe class API1
    {
        #region Basket
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceBasket(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends,
            double* price,
            double* ic);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceBasketAnyTime(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] past, // format [,]
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double[] trends,
            double nbTimeStep,
            double* price,
            double* ic);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultiCurrencyBasket(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            out IntPtr deltas
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultiCurrencyBasketAnyTime(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            double nbTimeStep,
            out IntPtr deltas
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasSingleCurrencyBasket(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            double[] FXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasSingleCurrencyBasketAnyTime(
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            double[] FXRates,
            double nbTimeStep,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );
        #endregion

        #region Multimonde2021 deprecated
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double[] trends,
            double* price,
            double* ic
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceMultimonde2021AnyTime(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            double* price,
            double* ic
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultiCurrencyMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            out IntPtr deltas
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultiCurrencyMultimonde2021AnyTime(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            out IntPtr deltas
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasSingleCurrencyMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends,
            double[] FXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasSingleCurrencyMultimonde2021AnyTime(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] current,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] trends,
            double[] currentFXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void TrackingErrorMultimonde(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlation,
            double[] FXRates,
            double[] trends,
            double* tracking_error
        );

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void ConvertDeltas(
            double[] deltas,
            double[] prices,
            double[] FXRates,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates
        );
        #endregion

        #region Utils
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe double call_pnl_cdfnor(double d);
        #endregion

        #region Quanto
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceQuanto(
            double maturity,
            double strike,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double[] interestRate,
            double[] correlations,
            double date,
            double[] currents,
            double* price,
            double* ic);
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]

        public static extern unsafe void SimulDeltasQuanto(
            double maturity,
            double strike,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double[] interestRate,
            double[] correlations,
            double date,
            double[] currents,
            out IntPtr deltasAssets,
            out IntPtr deltasFXRates);
        #endregion

        #region Multimonde 2021 Quanto
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceMultimonde2021Quanto(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations,
            double* price,
            double* ic);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultimonde2021Quanto(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations,
            out IntPtr deltas);

        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void DeltasMultimonde2021QuantoDebug(
            int sampleNumber,
            double[] past,
            int nbRows,
            double t,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations,
            out IntPtr deltas);
        #endregion
        #region SingleMonde
        [DllImport(@"..\..\..\..\x64\Debug\PricerDll.dll")]
        public static extern unsafe void PriceSingleMonde(int sampleNumber,
    //double past[], // format [,]
    double[] currentPrices,//taille 1, il s'agit juste du spot
    double[] volatilities,//taille 1 pareil
    double[] interestRates,//pour l'instant taille 1
    double* price,
    double T,
    double* ic);
        #endregion
    }
}