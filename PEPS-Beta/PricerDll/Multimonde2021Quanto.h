#pragma once
#include "Option.h"
class Multimonde2021Quanto :
	public Option
{
	PnlVect* payoffVectMemSpaceInit_;
	PnlVect* payoffVectMemSpaceCurrent_;
	double* interestRates;
public:
	Multimonde2021Quanto(double interestRates[6]);
	~Multimonde2021Quanto();

	double payoff(const PnlMat *path);
};

