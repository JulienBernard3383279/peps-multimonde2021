#pragma once
#include "Option.h"
class QuantoOption :
	public Option
{
	double strike_;
public:
	QuantoOption(double T, double strike);
	~QuantoOption();

	double payoff(const PnlMat *path);
};

