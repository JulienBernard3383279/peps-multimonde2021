#pragma region Init
#pragma once
#ifdef PRICERDLL_EXPORTS
#define PRICERDLL_API __declspec(dllexport)
#else
#define PRICERDLL_API __declspec(dllimport)
#endif
#pragma endregion

#pragma region Basket
extern "C" PRICERDLL_API void PriceBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double* price,
	double* ic);

extern "C" PRICERDLL_API void PriceBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double nbTimeStep,
	double* price,
	double* ic);

extern "C" PRICERDLL_API void DeltasMultiCurrencyBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double nbTimeStep,
	double** deltas
);

extern "C" PRICERDLL_API void DeltasMultiCurrencyBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[], 
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas
);

extern "C" PRICERDLL_API void DeltasSingleCurrencyBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates
);

extern "C" PRICERDLL_API void DeltasSingleCurrencyBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates
);
#pragma endregion

#pragma region Multimonde 2021 (deprecated)
extern "C" PRICERDLL_API void PriceMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double* price,
	double* ic
);

extern "C" PRICERDLL_API void PriceMultimonde2021AnyTime(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double* price,
	double* ic
);

extern "C" PRICERDLL_API void DeltasMultiCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas
);

extern "C" PRICERDLL_API void DeltasMultiCurrencyMultimonde2021AnyTime(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas
);

extern "C" PRICERDLL_API void DeltasSingleCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double currentFXRates[],
	double** deltasAssets,
	double** deltasFXRates
);

extern "C" PRICERDLL_API void DeltasSingleCurrencyMultimonde2021AnyTime(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double currentFXRates[],
	double** deltasAssets,
	double** deltasFXRates
);

extern "C" PRICERDLL_API void TrackingErrorMultimonde(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double FXRates[],
	double trends[],
	double* tracking_error
);

extern "C" PRICERDLL_API void ConvertDeltas(
	double deltas[],
	double prices[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates);
#pragma endregion

#pragma region Quanto
extern "C" PRICERDLL_API void PriceQuanto(
	double maturity,
	double strike,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double* price,
	double* ic);

extern "C" PRICERDLL_API void SimulDeltasQuanto(
	double maturity,
	double strike,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double** deltasAssets,
	double** deltasFXRates);
#pragma endregion

#pragma region Multimonde 2021 Quanto
extern "C" PRICERDLL_API void PriceMultimonde2021Quanto(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	double* price,
	double* ic);

extern "C" PRICERDLL_API void DeltasMultimonde2021Quanto(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	double** deltas);

extern "C" PRICERDLL_API void TrackingErrorMultimonde2021Quanto(
	int sampleNumber,
	double beginning,
	double end,
	double dateStartSimul,
	double providedScenario[],
	int nbRows,
	double spots[],
	double pricesAtSimulStart[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	int nbUpdates,
	double* trackingError,
	double* relativePricingVolatility,
	double** portfolioReturns,
	double** productReturns);
#pragma endregion

#pragma region SingleMonde
extern "C" PRICERDLL_API void PriceSingleMonde(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double* price,
	double* ic);

extern "C" PRICERDLL_API void DeltasSingleMonde(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double** deltasAssets,
	double** deltasFXRates);
#pragma endregion

#pragma region Utils
// FONCTIONS PNL EXPORTEES
extern "C" PRICERDLL_API double call_pnl_cdfnor(double x);
#pragma endregion