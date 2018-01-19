#include <ctime>

#include "stdafx.h"
#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"
#include "BasketOption.h"

#include "pnl/pnl_vector.h"

double Price(
	int optionType,
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation, //devra être une matrice
	int timestepCustom, //0 = basic, 1 = custom
	int timestepNumber, //osef si 1
	double timestepCustoms[],
	double trends[]) 
{
	PnlVect* payoffCoefficientsVect = pnl_vect_create_from_scalar(optionSize, 0.025);
	BasketOption* bo = new BasketOption(maturity, timestepNumber, optionSize, payoffCoefficientsVect, strike);

	PnlVect* volatilitiesVect = pnl_vect_create_from_scalar(optionSize, 0.2);
	PnlVect* spotsVect = pnl_vect_create_from_scalar(optionSize, 100.0);
	PnlVect* trendsVect = pnl_vect_create_from_scalar(optionSize, 0.0);
	BlackScholesModel* mod = new BlackScholesModel(optionSize, interestRate, correlation, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, 100002);
	MonteCarlo* mc = new MonteCarlo(mod, bo, rng, sampleNumber);

	double price, ic = 0;
	mc->price(price, ic);

	return price;
}