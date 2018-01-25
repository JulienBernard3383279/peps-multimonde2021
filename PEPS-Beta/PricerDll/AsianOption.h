#pragma once

#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

#include "Option.h"

class AsianOption : public Option
{
private:
	PnlVect * coefs_;
	double strike_;
	PnlVect* payoffVectMemSpace_;
	AsianOption();

public:
	AsianOption(double T, double nbTimeSteps, double size, PnlVect* coefs, double strike);
	~AsianOption();

	double payoff(const PnlMat *path);
};