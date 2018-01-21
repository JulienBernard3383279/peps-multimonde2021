
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
	double correlation[], //optionSize², traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
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
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(optionSize, optionSize, correlation);
	BlackScholesModel* mod;
	mod = new BlackScholesModel(optionSize, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

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
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[])
{
	MonteCarlo *mc;

	double temp = 371.0 / 365.25;
	double customDates[7] { 0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6 };
	Option *opt;
	opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	BlackScholesModel* mod;
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double price = 0;
	double ic = 0;
	mc->price(price, ic);

	return price;
}

double DeltasMultiCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[]) 
{
	/*
	* Pour obtenir le delta en "0" il faut shifter en "0+". Mathématiquement, on obtiendrait un delta
	* ne rendant pas compte du changement de performance mais juste de l'éventualité qu'on se heurte au mur des 15%.
	* C'est le cas pour toutes les dates de constation, on s'intéresse à l'instant suivant.
	* Exemple, avec une option qui donne la performance de l'option entre 0 et T (ie prix de l'actif en T / prix
	* de l'actif en 0), si on shift en 0 et non 0+, alors le delta obtenu pour cet actif par rapport à son sous-jacent
	* serait de 0 alors qu'il est évidemment de 1. - Julien
	*/

	MonteCarlo *mc;

	double temp = 371.0 / 365.25;
	double customDates[7]{ 0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6 };
	Option *opt;
	opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	BlackScholesModel* mod;
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double *deltas;
	mc->deltas(deltas);

	return 0.0; //TODO
}

double DeltasSingleCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double FXRates[])
{
	MonteCarlo *mc;

	double temp = 371.0 / 365.25;
	double customDates[7]{ 0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6 };
	Option *opt;
	opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	BlackScholesModel* mod;
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double *deltas;
	mc->deltas(deltas);

	double *deltasIndicesXRates = new double[6];
	double *deltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		deltasIndicesXRates[i] = deltas[i] / FXRates[i];
		deltasFXRates[i] = -deltas[i] * spots[i] / FXRates[i];
	}

	return 0.0; //TODO
}