#pragma once

#ifdef PRICERDLL_EXPORTS
#define PRICERDLL_API __declspec(dllexport)
#else
#define PRICERDLL_API __declspec(dllimport)
#endif

extern "C" PRICERDLL_API double PriceBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation,
	int timestepNumber,
	double trends[]);

extern "C" PRICERDLL_API double PriceMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation,
	double trends[]
);
