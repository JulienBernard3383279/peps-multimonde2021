#pragma once

#include "stdafx.h"
#include "pnl/pnl_vector.h"
#include <cmath>
#include <iostream>

static double ComputeValue(PnlVect* quantities, PnlVect* values) {
	double sum = 0;
	for (int i = 0; i < quantities->size; i++) {
		sum += GET(quantities, i)*GET(values, i);
	}
	return sum;
}

static void UpdateCurrencyQuantities(double timeGap,
	double* localQuantity,// double localInterestRate,
	int startIndex, PnlVect* quantities, PnlVect* interestRates) {
	//Seules les quantités 6-10 sont augmentées ( les monnaies )
	*localQuantity *= exp(1 + GET(interestRates,0) * timeGap); 
	for (int i = startIndex; i < startIndex + quantities->size; i++) {
		LET(quantities, i) *= exp(GET(interestRates, i-startIndex)*timeGap);
	}
}

static void UpdateCurrencyQuantities(double timeGap,
	double* localQuantity,// double localInterestRate,
	int startIndex, PnlVect* quantities, double interestRates[]) {
	//Seules les quantités 6-10 sont augmentées ( les monnaies )
	*localQuantity *= exp(interestRates[0]*timeGap);
	for (int i = startIndex; i < quantities->size; i++) {
		LET(quantities, i) *= exp(interestRates[1 + i - startIndex] * timeGap);
	}
}

static void UpdateLocalCurrencyQuantity(double timeGap, double *localQuantity, double interestRates[]) {
	*localQuantity *= exp(interestRates[0] * timeGap);
}

static void UpdatePortfolio(PnlVect* quantities, PnlVect* values, PnlVect* deltas, double &spare) {
	double budget = spare + ComputeValue(quantities, values);
	double cost = ComputeValue(deltas, values);
	spare = budget - cost;
	for (int i = 0; i < quantities->size; i++) {
		LET(quantities, i) = GET(deltas, i);
	}
}

static void UpdatePortfolio(PnlVect* quantities, PnlVect* values, PnlVect* deltas) {
	for (int i = 0; i < quantities->size; i++) {
		LET(quantities, i) = GET(deltas, i);
	}
}