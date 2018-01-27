#include "stdafx.h"
#include "ProfitAndLossUtilities.h"
#include <cmath>
#include <iostream>


ProfitAndLossUtilities::ProfitAndLossUtilities()
{
}


ProfitAndLossUtilities::~ProfitAndLossUtilities()
{
}

// This function updates all deltas to match hedge our option, as well as matching our portfolio value
void ProfitAndLossUtilities::updateDeltas(PnlVect * asset_deltas, PnlVect* fx_deltas, const PnlVect * FXRate, const PnlVect * spotsVect, const double port_value)
{
	pnl_vect_div_vect_term(asset_deltas, FXRate);

	pnl_vect_clone(fx_deltas, asset_deltas);
	pnl_vect_mult_vect_term(fx_deltas, spotsVect);
	pnl_vect_minus(fx_deltas);

	LET(fx_deltas, 0) = 0;
	PnlVect* prices_in_euro = pnl_vect_copy(spotsVect);
	pnl_vect_mult_vect_term(prices_in_euro, FXRate);
	LET(fx_deltas, 0) = port_value - pnl_vect_scalar_prod(prices_in_euro, asset_deltas) - pnl_vect_scalar_prod(fx_deltas, FXRate);
}

void ProfitAndLossUtilities::updatePortfolioValue(double& port_value, double timestep, const PnlVect* asset_deltas, PnlVect* fx_deltas, const PnlVect * FXRate, const PnlVect * taux, const PnlVect * spotsVect)
{
	PnlVect* prices_in_euro = pnl_vect_copy(spotsVect);
	pnl_vect_mult_vect_term(prices_in_euro, FXRate);

	int nbAssets = fx_deltas->size;

	for (int i = 0; i < nbAssets; ++i) {
		LET(fx_deltas, i) *= exp(GET(taux, i)*timestep);
	}


	port_value = pnl_vect_scalar_prod(prices_in_euro, asset_deltas) + pnl_vect_scalar_prod(fx_deltas, FXRate);
}
