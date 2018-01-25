
#include "stdafx.h"

#include "time.h"
#include <iostream>

#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"

#include "BasketOption.h"
#include "Multimonde2021.h"

#include "pnl/pnl_vector.h"

void PriceBasket(
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
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	
	PnlVect* payoffCoefficientsVect = pnl_vect_create_from_ptr(optionSize, payoffCoefficients);
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

	mc->price(price, ic);
}

void PriceMultimonde2021(
	int sampleNumber,
	double spots[], //currently unused
	double volatilities[], //currently unused
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* price,
	double* ic)
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

	mc->price(price, ic);

}

void DeltasMultiCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas)
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
	//pnl_rng_sseed(rng, time(NULL));
	pnl_rng_sseed(rng, 0);

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	// https://stackoverflow.com/questions/36224120/array-from-c-to-c-sharp
	double* myDeltas = new double[6];
	mc->deltas(myDeltas);

	*deltas = static_cast<double*>(malloc(6*sizeof(double)));
	memcpy(*deltas, &(myDeltas[0]), 6*sizeof(double));
}

void DeltasSingleCurrencyMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates)
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

	double* deltas = new double[6];
	mc->deltas(deltas);

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = deltas[i] / FXRates[i];
		myDeltasFXRates[i] = -deltas[i] * spots[i] / FXRates[i];
	}

	*deltasAssets = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 6 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 6 * sizeof(double));
}

void TrackingErrorMultimonde(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* tracking_error)
{
	/*
	double ic = 0.;
	double price = 0.;

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
	mod->initAsset(opt->nbTimeSteps);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);


	PnlMat* simulated_prices = pnl_mat_create_from_double(2227 + 1, opt->size, 0.);
	PnlMat* FX_prices = pnl_mat_create_from_double(2227 + 1, opt->size, 1.);

	mod->postInitAsset(simulated_prices, (int)(temp * 6), 2227, rng);

	PnlMat* current_prices = pnl_mat_create(0, opt->size);

	double t = 0.;
	double T = temp * 6;
	double port = 0.;
	double *deltas;
	PnlVect* asset_deltas = pnl_vect_create_from_double(6, 0.);
	PnlVect* riskfree_deltas = pnl_vect_create_from_double(6, 0.);

	mc->price(&port, &ic);
	mc->deltas(asset_deltas);
	rf_deltas(riskfree_deltas, asset_deltas, port);

	while (t + 1 < T) {
		t+=1;
		pnl_mat_add_row(current_prices,(int) t, spotsVect);
	}
	*/
}