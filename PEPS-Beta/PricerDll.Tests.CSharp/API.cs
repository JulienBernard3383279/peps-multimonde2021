using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PricerConsole
{
    public static unsafe class API
    {
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
            int timestepNumber,
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
            int timestepNumber,
            double[] trends,
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
            IntPtr deltas
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
            double[] interestRate,
            double[] correlation,
            double[] trends,
            IntPtr deltas
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
            IntPtr deltasAssets,
            IntPtr deltasFXRates
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
            IntPtr deltasAssets,
            IntPtr deltasFXRates
        );












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
    }
}