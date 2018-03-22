#pragma once
#include "Option.h"

class SingleMonde :
	public Option
{
	

public:
	SingleMonde(double T);
	~SingleMonde();

	double payoff(const PnlMat* path);

	double verbosePayoff();
};
