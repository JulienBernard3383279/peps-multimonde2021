#pragma once
#include "Option.h"

class SingleMonde :
	public Option
{
	double payoff;
public:
	SingleMonde(double T);
	~SingleMonde();

	double payoff(const PnlMat *path);

	double verbosePayoff(const PnlMat *path);
};
