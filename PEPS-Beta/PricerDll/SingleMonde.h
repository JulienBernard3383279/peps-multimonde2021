#pragma once
#include "Option.h"

class SingleMonde :
	public Option
{
	

public:
	SingleMonde();
	~SingleMonde();

	double payoff(const PnlMat* path);

	double verbosePayoff();
};
