#pragma once
#include "Option.h"
class Multimonde2021Quanto :
	public Option
{
	PnlVect* payoffVectMemSpaceInit_;
	PnlVect* payoffVectMemSpaceCurrent_;
public:
	Multimonde2021Quanto();
	~Multimonde2021Quanto();

	double payoff(const PnlMat *path);
};

