#pragma once

#include "pnl/pnl_vector.h"

class ProfitAndLossUtilities
{
public:
	ProfitAndLossUtilities();
	~ProfitAndLossUtilities();

	void updateDeltas(PnlVect* asset_deltas, PnlVect* fx_deltas, const PnlVect* FXRate, const PnlVect* spotsVect, const double port_value);

	void updatePortfolioValue(double& port_value, double timestep, const PnlVect* asset_deltas, PnlVect* fx_deltas, const PnlVect * FXRate, const PnlVect * taux, const PnlVect * spotsVect);
};

