#pragma once

#ifdef PRICERDLL_EXPORTS
#define PRICERDLL_API __declspec(dllexport)
#else
#define PRICERDLL_API __declspec(dllimport)
#endif

extern "C" PRICERDLL_API double Price(
	int optionType,
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation, //devra être une matrice
	int timestepCustom, //0 = basic, 1 = custom
	int timestepNumber, //osef si 1
	double timestepCustoms[],
	double trends[]);
