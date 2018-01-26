#include "stdafx.h"

#include "time.h"
#include <iostream>

#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"

#include "BasketOption.h"
#include "Multimonde2021.h"

#include "pnl/pnl_vector.h"
#include "pnl/pnl_cdf.h"

#include "ProfitAndLossUtilities.h"

void InitBasket(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[])
{
	PnlVect* payoffCoefficientsVect = pnl_vect_create_from_ptr(optionSize, payoffCoefficients);
	*opt = new BasketOption(maturity, 1, optionSize, payoffCoefficientsVect, strike);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(optionSize, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(optionSize, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(optionSize, trends);
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(optionSize, optionSize, correlation);
	*mod = new BlackScholesModel(optionSize, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}

void InitBasketAnyTime(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[])
{
	PnlVect* payoffCoefficientsVect = pnl_vect_create_from_ptr(optionSize, payoffCoefficients);
	*opt = new BasketOption(maturity, 1, optionSize, payoffCoefficientsVect, strike);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(optionSize, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_zero(optionSize);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(optionSize, trends);
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(optionSize, optionSize, correlation);
	*mod = new BlackScholesModel(optionSize, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}

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
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasket(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	mc->price(price, ic);
}

void PriceBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double current[], 
	double volatilities[],
	double interestRate,
	double correlation[], //optionSize², traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasketAnyTime(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestion paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra être multidimensionnel [,], pas jagged !
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	mc->price(pastMat, t, currentVect, price, ic);
}

void DeltasMultiCurrencyBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double** deltas)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasket(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}

	*deltas = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltas, &(intermediate[0]), 6 * sizeof(double));
}

void DeltasMultiCurrencyBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasketAnyTime(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestion paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra être multidimensionnel [,], pas jagged !
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}

	*deltas = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltas, &(intermediate[0]), 6 * sizeof(double));
}

void DeltasSingleCurrencyBasket(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasket(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}

	PnlVect* deltas = deltas = pnl_vect_create_from_zero(6);
	mc->deltas(deltas);

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = GET(deltas, i) / FXRates[i];
		myDeltasFXRates[i] = -GET(deltas, i) * spots[i] / FXRates[i];
	}

	*deltasAssets = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 6 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 6 * sizeof(double));
}

void DeltasSingleCurrencyBasketAnyTime(
	double maturity,
	int optionSize,
	double strike,
	double payoffCoefficients[],
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasketAnyTime(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestion paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra être multidimensionnel [,], pas jagged !
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	/*PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}*/

	PnlVect* deltas = deltas = pnl_vect_create_from_zero(6);
	mc->deltas(deltas);

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = GET(deltas, i) / FXRates[i];
		myDeltasFXRates[i] = -GET(deltas, i) * current[i] / FXRates[i];
	}

	*deltasAssets = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 6 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 6 * sizeof(double));
}













void InitMultimonde2021(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[])
{
	double temp = 371.0 / 365.25;
	PnlVect* customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	*opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	*mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}

void InitMultimonde2021AnyTime(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	int sampleNumber,
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[])
{
	double temp = 371.0 / 365.25;
	PnlVect* customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	*opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_zero(6);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	*mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}


void PriceMultimonde2021(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021(&mc, &opt, &mod, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	mc->price(price, ic);

}

void PriceMultimonde2021AnyTime(
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double current[],
	double volatilities[],
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021AnyTime(&mc, &opt, &mod, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestions paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra être multidimensionnel [,], pas jagged !
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	mc->price(pastMat, t, currentVect, price, ic);
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
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021(&mc, &opt, &mod, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}

	*deltas = static_cast<double*>(malloc(6*sizeof(double)));
	memcpy(*deltas, &(intermediate[0]), 6*sizeof(double));
}

void DeltasMultiCurrencyMultimonde2021AnyTime(
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double current[], 
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double** deltas)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021AnyTime(&mc, &opt, &mod, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestions paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past);
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	PnlVect* myDeltas = pnl_vect_create_from_zero(6);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}

	*deltas = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltas, &(intermediate[0]), 6 * sizeof(double));
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
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021(&mc, &opt, &mod, sampleNumber, spots, volatilities, interestRate, correlation, trends);

	PnlVect* deltas = deltas = pnl_vect_create_from_zero(6);
	mc->deltas(deltas);

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = GET(deltas,i) / FXRates[i];
		myDeltasFXRates[i] = -GET(deltas,i) * spots[i] / FXRates[i];
	}

	*deltasAssets = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 6 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 6 * sizeof(double));
}

void DeltasSingleCurrencyMultimonde2021AnyTime(
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double current[], 
	double volatilities[],
	double interestRate,
	double correlation[],
	double trends[],
	double currentFXRates[],
	double** deltasAssets,
	double** deltasFXRates)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021AnyTime(&mc, &opt, &mod, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestions paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past);
	PnlVect* currentVect = pnl_vect_create_from_ptr(6, current);

	PnlVect* deltas = pnl_vect_create_from_zero(6);
	mc->deltas(pastMat, t, currentVect, deltas);

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = GET(deltas, i) / currentFXRates[i];
		myDeltasFXRates[i] = -GET(deltas, i) * current[i] / currentFXRates[i];
	}

	*deltasAssets = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 6 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 6 * sizeof(double));
}

void ConvertDeltas(
	double deltas[],
	double prices[],
	double FXRates[],
	double** deltasAssets,
	double** deltasFXRates) {

	double* myDeltasAssets = new double[6];
	double* myDeltasFXRates = new double[6];

	for (int i = 0; i < 6; i++) {
		myDeltasAssets[i] = deltas[i] / FXRates[i];
		myDeltasFXRates[i] = - deltas[i] * prices[i] / FXRates[i];
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
	double FXRates[],
	double* tracking_error)
{
	double ic = 0.;
	double price = 0.;

	MonteCarlo *mc;
	std::cout << "Bug0" << std::endl;

	double temp = 371.0 / 365.25;
	PnlVect* customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	Option *opt;
	opt = new Multimonde2021(customDates);

	std::cout << "Bug1" << std::endl;

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, FXRates);

	PnlVect* volatilitiesVectHisto = pnl_vect_create_from_ptr(11, volatilities);
	PnlVect* spotsVectHisto = pnl_vect_create_from_ptr(11, spots);
	PnlVect* trendsVectHisto = pnl_vect_create_from_zero(11);
	for (int i = 0; i < 6; ++i) {
		LET(trendsVectHisto, i) = GET(trendsVect, i);
	}
	for (int i = 1; i < 6; ++i) {
		LET(trendsVectHisto, i + 5) = GET(trendsVect, i) - GET(trendsVect, 0);
	}

	std::cout << "Bug2" << std::endl;

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	// Reflechir sur la matrice de correlation de la fonction
	BlackScholesModel* mod;
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	BlackScholesModel* modDonnees;
	PnlMat* correlationsMatHisto = pnl_mat_create_from_ptr(11, 11, correlation);
	modDonnees = new BlackScholesModel(11, interestRate, correlationsMatHisto, volatilitiesVectHisto, spotsVectHisto, trendsVectHisto);
	modDonnees->initAsset(2227);

	PnlMat* simulated_prices = pnl_mat_create_from_double(2227 + 1, opt->size * 2 - 1, 0.);
	mod->postInitAsset(simulated_prices, (int)(temp * 6), 2227, rng);

	PnlMat* asset_prices = pnl_mat_create_from_double(2227 + 1, opt->size, 1.);
	PnlMat* FX_prices = pnl_mat_create_from_double(2227 + 1, opt->size - 1, 1.);
	pnl_mat_extract_subblock(asset_prices, simulated_prices, 0, 6, 0, 2228);
	pnl_mat_extract_subblock(FX_prices, simulated_prices, 6, 5, 0, 2228);

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	PnlVect* current_prices = pnl_vect_create_from_zero(opt->size);
	PnlVect* current_fx_rates = pnl_vect_create_from_double(opt->size, 1.);

	std::cout << "Bug3" << std::endl;

	int t = 0;
	int T = 371 * 6;
	double port = 0.;

	PnlVect* asset_deltas = pnl_vect_create_from_zero(6);
	PnlVect* fx_deltas = pnl_vect_create_from_zero(6);
	mc->price(&port, &ic);
	mc->deltas(asset_deltas);

	ProfitAndLossUtilities* pnlUtility = new ProfitAndLossUtilities();
	pnlUtility->updateDeltas(asset_deltas, fx_deltas, trendsVect, spotsVect, port);

	PnlMat* past = pnl_mat_create_from_zero(1, opt->size);
	pnl_mat_set_row(past, current_prices, 0);
	std::cout << "Bug4" << std::endl;


	double timestep = 1. / 365.25;
	while (t + 1 < T) {
		t += 1;
		pnl_mat_get_row(current_prices, asset_prices, t);
		pnl_mat_get_row(current_fx_rates, FX_prices, t);

		pnlUtility->updatePortfolioValue(port, timestep, asset_deltas, fx_deltas, current_fx_rates, current_prices);
		if (t % 371 == 0) {
			pnl_mat_add_row(past, past->m, current_prices);
		}
		mc->deltas(past, t*timestep, current_prices, asset_deltas);
		pnlUtility->updateDeltas(asset_deltas, fx_deltas, trendsVect, current_prices, port);
	}

	tracking_error = 0;
	//*tracking_error = opt->payoff(past) - port;
}

	
double call_pnl_cdfnor(double x) {
	return pnl_cdfnor(x);
}