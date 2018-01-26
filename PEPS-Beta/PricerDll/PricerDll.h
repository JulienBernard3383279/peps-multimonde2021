#pragma once

#ifdef PRICERDLL_EXPORTS
#define PRICERDLL_API __declspec(dllexport)
#else
#define PRICERDLL_API __declspec(dllimport)
#endif

// TODO FACTORISATION SERIEUSE A FAIRE POST RENDU BETA

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

extern "C" PRICERDLL_API void ConvertDeltas(
	double deltas[],
	double prices[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates);

// FONCTIONS PNL EXPORTEES

extern "C" PRICERDLL_API double call_pnl_cdfnor(double x);