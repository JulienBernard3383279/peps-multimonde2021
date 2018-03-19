#pragma once

#include "stdafx.h"
#include "pnl/pnl_vector.h"
#include <cmath>

static double ComputeValue(PnlVect* quantities, PnlVect* values) {
	double sum = 0;
	for (int i = 0; i < quantities->size; i++) {
		sum += GET(quantities, i)*GET(values, i);
	}
	return sum;
}

static void UpdateCurrencyQuantities(double timeGap,
	double &localQuantity, double localInterestRate,
	PnlVect* quantities, PnlVect* interestRates) {
	//Il faudra mapper les valeurs par référence lors de la construction du vecteur quantité 6-10 (wrap ?)
	localQuantity *= exp(1 + localInterestRate * timeGap); 
	for (int i = 0; i < quantities->size; i++) {
		LET(quantities, i) *= exp(1 + GET(interestRates, i)*timeGap);
	}
}

static void UpdatePortfolio(PnlVect* quantities, PnlVect* values, PnlVect* deltas, double &spare) {
	double budget = spare + ComputeValue(quantities, values);
	double cost = ComputeValue(deltas, values);
	spare = budget - cost;
	for (int i = 0; i < quantities->size; i++) {
		LET(quantities, i) = GET(deltas, i);
	}
}