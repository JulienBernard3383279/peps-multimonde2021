#pragma once
#include "Option.h"

class SingleMonde :
	public Option
{
	double* interestRates;
public:
	SingleMonde(double interestRates[2]);
	~SingleMonde();

	double payoff(const PnlMat* path);

	double verbosePayoff();
};
