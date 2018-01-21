
#include "stdafx.h"

#include "time.h"

#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"

#include "BasketOption.h"
#include "Multimonde2021.h"

#include "pnl/pnl_vector.h"

double PriceBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation, //devra être une matrice
	int timestepNumber, //osef si 1
	double trends[])
{
	MonteCarlo *mc;
	
	PnlVect* payoffCoefficientsVect = pnl_vect_create_from_ptr(optionSize, payoffCoefficients);
	double temp;
	Option *opt;
	opt = new BasketOption(maturity, timestepNumber, optionSize, payoffCoefficientsVect, strike);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(optionSize, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(optionSize, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(optionSize, trends);
	BlackScholesModel* mod;
	mod = new BlackScholesModel(optionSize, interestRate, correlation, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double price = 0;
	double ic = 0;
	mc->price(price, ic);

	return price;
}

double PriceMultimonde2021(
	int sampleNumber,
	double spots[], //currently unused
	double volatilities[], //currently unused
	double interestRate,
	double correlation, // will have to be a matrix
	double trends[]
) {
	MonteCarlo *mc;

	double temp = 371.0 / 365.25;
	double customDates[7] { 0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6 };
	Option *opt;
	opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	BlackScholesModel* mod;
	mod = new BlackScholesModel(6, interestRate, correlation, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double price = 0;
	double ic = 0;
	mc->price(price, ic);

	return price;
}