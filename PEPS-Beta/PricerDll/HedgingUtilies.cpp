#pragma once

#include "stdafx.h"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
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

static void InterpolateValues(PnlVect* receiver, PnlMat* path, double t, double T, int entries) {
	double floatEntryIndex = (t / T * entries);
	double closeness = (floatEntryIndex) - (int) (floatEntryIndex);
	if (closeness > -0.0001 && closeness < 0.0001) {
		std::cout << "YUP " << floatEntryIndex << " ; " << closeness << std::endl;
		pnl_mat_get_row(receiver, path, (int)(floatEntryIndex + 0.0001));
	}
	else {
		std::cout << "NOPE " << floatEntryIndex << " ; " << closeness << std::endl;
		PnlVect* temp1 = pnl_vect_create(receiver->size);
		pnl_mat_get_row(temp1, path, (int)(floatEntryIndex));
		PnlVect* temp2 = pnl_vect_create(receiver->size);
		pnl_mat_get_row(temp1, path, (int)(floatEntryIndex)+1);
		pnl_vect_mult_scalar(temp1, 1 - (floatEntryIndex - (int)floatEntryIndex));
		pnl_vect_mult_scalar(temp2, (floatEntryIndex - (int)floatEntryIndex));
		pnl_vect_plus_vect(temp1, temp2);
		pnl_vect_clone(receiver, temp1);
		pnl_vect_free(&temp1);
		pnl_vect_free(&temp2);
	}
}