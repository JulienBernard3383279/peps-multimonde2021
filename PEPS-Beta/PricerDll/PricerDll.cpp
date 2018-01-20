#include <ctime>

#include "stdafx.h"
#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"

#include "BasketOption.h"
#include "Multimonde2021.h"

#include "pnl/pnl_vector.h"

double Price(
	int optionType, //0 = multimonde ; 1 = basket
	double maturity, // doesn't matter if timestepCustom
	int optionSize, 
	double strike, // if applicable
	double payoffCoefficients[], //currently unused
	int sampleNumber,
	double spots[], //currently unused
	double volatilities[], //currently unused
	double interestRate,
	double correlation, // will have to be a matrix
	int timestepCustom, //0 = basic, 1 = custom
	int timestepNumber, // doesn't matter if 1
	double timestepCustoms[], //doesn't matter if 0
	double trends[]) 
{
	MonteCarlo *mc;
	
	PnlVect * payoffCoefficientsVect = pnl_vect_create_from_scalar(optionSize, 0.025);
	double temp;
	Option *opt;
	switch (optionType) {
	case 1:
		opt = new BasketOption(maturity, timestepNumber, optionSize, payoffCoefficientsVect, strike);
		break;
	case 0:
	default:
		temp = 371.0 / 365.25;
		opt = new Multimonde2021(new double[7] { 0, temp, 2 * temp, 3 * temp, 4 * temp, 5 * temp, 6 * temp });
		break;
	}

	PnlVect* volatilitiesVect = pnl_vect_create_from_scalar(optionSize, 0.2);
	PnlVect* spotsVect = pnl_vect_create_from_scalar(optionSize, 100.0);
	PnlVect* trendsVect = pnl_vect_create_from_scalar(optionSize, 0.0);
	BlackScholesModel* mod;
	mod = new BlackScholesModel(optionSize, interestRate, correlation, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, 100002);
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double price = 0;
	double ic = 0;
	mc->price(price, ic);

	return price;
}