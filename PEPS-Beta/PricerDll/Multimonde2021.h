#pragma once
#include "Option.h"

class Multimonde2021 :
	public Option
{
	PnlVect* payoffVectMemSpaceInit_;
	PnlVect* payoffVectMemSpaceCurrent_;
public:
	Multimonde2021(PnlVect* customDates);
	~Multimonde2021();

	double payoff(const PnlMat *path);

	double verbosePayoff(const PnlMat *path);
};


