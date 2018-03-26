#pragma once

#pragma region Includes
#include "stdafx.h"

#include "time.h"
#include <iostream>

#include "PricerDll.h"

#include "BlackScholesModel.h"
#include "MonteCarlo.h"
#include "QuantoOption.h"
#include "BasketOption.h"
#include "Multimonde2021.h"
#include "Multimonde2021Quanto.h"
#include "SingleMonde.h"

#include "pnl/pnl_vector.h"
#include "pnl/pnl_cdf.h"

#include "ProfitAndLossUtilities.h"

#include "MathUtils.cpp"
#include "HedgingUtilies.cpp"

#pragma endregion

#pragma region Basket Option
#pragma region Inits
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
#pragma endregion
#pragma region Price
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
#pragma endregion
#pragma region Deltas multi currency
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
#pragma endregion
#pragma region Deltas single currency
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

	PnlVect* deltas = deltas = pnl_vect_create_from_zero(2);
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
	mc->deltas(pastMat, t, currentVect, deltas);

	double* intermediate = new double[6];
	for (int i = 0; i < 6; i++) {
		intermediate[i] = GET(myDeltas, i);
	}*/

	PnlVect* deltas = deltas = pnl_vect_create_from_zero(6);
	mc->deltas(pastMat, t, currentVect, deltas);

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
#pragma endregion
#pragma endregion

#pragma region Multimonde 2021 (deprecated)
#pragma region Inits
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
#pragma endregion
#pragma region Price
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
#pragma endregion
#pragma region Deltas multi currency
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
#pragma endregion
#pragma region Deltas single currency
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
#pragma endregion
#pragma region Convert deltas
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
#pragma endregion
#pragma region Tracking error
void TrackingErrorMultimonde(
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate,
	double correlation[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double FXRates[],
	double trends[],
	double* tracking_error)
{
	double ic = 0.;
	double price = 0.;

	MonteCarlo *mc;

	double temp = 371.0 / 365.25;
	PnlVect* customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	Option *opt;
	opt = new Multimonde2021(customDates);

	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(6, volatilities);
	PnlVect* spotsVect = pnl_vect_create_from_ptr(6, spots);
	PnlVect* trendsVect = pnl_vect_create_from_ptr(6, trends);

	PnlVect* volatilitiesVectHisto = pnl_vect_create_from_ptr(11, volatilities);
	PnlVect* spotsVectHisto = pnl_vect_create_from_ptr(11, spots);
	PnlVect* trendsVectHisto = pnl_vect_create_from_zero(11);

	PnlVect* fx_taux = pnl_vect_create_from_zero(6);

	for (int i = 0; i < 6; ++i) {
		LET(trendsVectHisto, i) = GET(trendsVect, i);
	}
	for (int i = 1; i < 6; ++i) {
		LET(spotsVectHisto, i + 5) = FXRates[i];
		LET(trendsVectHisto, i + 5) = GET(trendsVect, i) - GET(trendsVect, 0);
		LET(fx_taux, i) = GET(trendsVect, i) - GET(trendsVect, 0);
	}
	LET(fx_taux, 0) = interestRate;

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	// Reflechir sur la matrice de correlation de la fonction
	BlackScholesModel* mod;
	PnlMat* correlationsMat = pnl_mat_create_from_ptr(6, 6, correlation);
	mod = new BlackScholesModel(6, interestRate, correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	BlackScholesModel* modDonnees;
	PnlMat* correlationsMatHisto = pnl_mat_create_from_zero(11, 11);
	pnl_mat_set_subblock(correlationsMatHisto, correlationsMat, 0, 0);
	modDonnees = new BlackScholesModel(11, interestRate, correlationsMatHisto, volatilitiesVectHisto, spotsVectHisto, trendsVectHisto);
	modDonnees->initAsset(2227);
	
	PnlMat* simulated_prices = pnl_mat_create_from_double(2227, opt->size * 2 - 1, 0.);
	
	modDonnees->postInitAsset(simulated_prices, temp * 6, 2226, rng);
	
	PnlMat* asset_prices = pnl_mat_create_from_double(2227, opt->size, 1.);
	PnlMat* FX_prices = pnl_mat_create_from_double(2227, opt->size - 1, 1.);
	
	pnl_mat_extract_subblock(asset_prices, simulated_prices, 0, 2227, 0, 6);
	pnl_mat_extract_subblock(FX_prices, simulated_prices, 0, 2227, 6, 5);
	
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);
	
	PnlVect* current_prices = pnl_vect_create_from_zero(opt->size);
	PnlVect* tmp_current_fx_rates = pnl_vect_create_from_double(opt->size-1, 1.);
	PnlVect* current_fx_rates = pnl_vect_create_from_double(opt->size-1, 1.);

	int t = 0;
	int T = 371 * 6;
	double port = 0.;

	pnl_mat_get_row(current_prices, asset_prices, 0);

	pnl_mat_get_row(tmp_current_fx_rates, FX_prices, t);
	current_fx_rates = pnl_vect_create_from_zero(5);
	pnl_vect_clone(current_fx_rates, tmp_current_fx_rates);
	pnl_vect_resize(current_fx_rates, current_fx_rates->size + 1);
	for (int i = current_fx_rates->size - 1; i >0; i--) {
		LET(current_fx_rates, i) = GET(current_fx_rates, i - 1);
	}
	LET(current_fx_rates, 0) = 1.;

	PnlVect* asset_deltas = pnl_vect_create_from_zero(6);
	PnlVect* fx_deltas = pnl_vect_create_from_zero(6);
	mc->price(&port, &ic);
	mc->deltas(asset_deltas);

	ProfitAndLossUtilities* pnlUtility = new ProfitAndLossUtilities();
	pnlUtility->updateDeltas(asset_deltas, fx_deltas, current_fx_rates, spotsVect, port);

	PnlMat* past = pnl_mat_create_from_zero(1, opt->size);
	pnl_mat_set_row(past, current_prices, 0);

	double timestep = 1. / 365.25;

	pnl_mat_print(FX_prices);

	while (t < T) {
		t += 1;
		pnl_mat_get_row(current_prices, asset_prices, t);

		pnl_mat_get_row(tmp_current_fx_rates, FX_prices, t);
		current_fx_rates = pnl_vect_create_from_zero(5);
		pnl_vect_clone(current_fx_rates, tmp_current_fx_rates);
		pnl_vect_resize(current_fx_rates, current_fx_rates->size + 1);
		for (int i = current_fx_rates->size-1; i >0; i--) {
			LET(current_fx_rates, i) = GET(current_fx_rates, i - 1);
		}
		LET(current_fx_rates, 0) = 1.;

		pnlUtility->updatePortfolioValue(port, timestep, asset_deltas, fx_deltas, current_fx_rates, fx_taux, current_prices);

		if (t % 371 == 0) {
			pnl_mat_add_row(past, past->m, current_prices);
		}
		if (t < T) {
			for (int i = 0; i < opt->size; ++i) {
				LET(asset_deltas, i) = 0.;
			}
			mc->deltas(past, t*timestep, current_prices, asset_deltas);
			pnlUtility->updateDeltas(asset_deltas, fx_deltas, current_fx_rates, current_prices, port);
		}
	}
	*tracking_error = opt->payoff(past) - port;

	pnl_mat_free(&past);
	pnl_mat_free(&simulated_prices);
	pnl_mat_free(&FX_prices);
	pnl_vect_free(&current_prices);
	pnl_vect_free(&current_fx_rates);
	pnl_vect_free(&asset_deltas);
	pnl_vect_free(&fx_deltas);
	pnl_vect_free(&fx_taux);
	pnl_vect_free(&tmp_current_fx_rates);
	pnl_vect_free(&asset_deltas);


}
#pragma endregion
#pragma endregion

#pragma region Quanto Option
#pragma region Price
void PriceQuanto(
	double maturity,
	double strike,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	opt = new QuantoOption(maturity, strike);

	// Calcul de la vol de S-X
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(2, volatilities);
	LET(volatilitiesVect, 0) = VolAminusB(correlations[1], volatilities[0], volatilities[1]);

	// Calcul de la cor de S-X et -X
	PnlMat* correlationsMat = GenCorrAMinusBBFromCorrAB(correlations, volatilities[0], volatilities[1]);
	ReverseCorrMatrix(correlationsMat);//car on nous passe celle de S$ avec €/$ et qu'on veut celle de S€ avec $/€

									   // On fixe les mu avec les taux sans risque
	PnlVect* trendsVect = pnl_vect_create_from_zero(2);
	LET(trendsVect, 0) = interestRate[0];
	LET(trendsVect, 1) = interestRate[0];
	
	// On actualise le prix en euros
	PnlVect* spotsVect = pnl_vect_create_from_ptr(2, spots);
	LET(spotsVect, 0) *= GET(spotsVect, 1) / exp(-interestRate[1] * maturity);
	
	PnlMat* past = pnl_mat_create_from_zero(1, 2);
	pnl_mat_set_row(past, spotsVect, 0);

	PnlVect* currVect = pnl_vect_create_from_ptr(2, currents);
	LET(currVect, 0) *= GET(currVect, 1) / exp(-interestRate[1] * (maturity - date));

	mod = new BlackScholesModel(2, interestRate[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	mc->price(past, date, currVect, price, ic);
}

#pragma endregion
#pragma region simulDeltas
void SimulDeltasQuanto(
	double maturity,
	double strike,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double date,
	double currents[],
	double** deltasAssets,
	double** deltasFXRates)
{

	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	opt = new QuantoOption(maturity, strike);

	// Calcul de la vol de S-X
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(2, volatilities);
	LET(volatilitiesVect, 0) = VolAminusB(correlations[1], volatilities[0], volatilities[1]);

	// Calcul de la cor de S-X et -X
	PnlMat* correlationsMat = GenCorrAMinusBBFromCorrAB(correlations, volatilities[0], volatilities[1]);
	ReverseCorrMatrix(correlationsMat);//car on nous passe celle de S$ avec €/$ et qu'on veut celle de S€ avec $/€

	// On actualise le prix en euros
	PnlVect* spotsVect = pnl_vect_create_from_ptr(2, spots);
	LET(spotsVect, 0) *= GET(spotsVect, 1) / exp(-interestRate[1] * maturity);
	PnlMat* past = pnl_mat_create_from_zero(1, 2);
	pnl_mat_set_row(past, spotsVect, 0);


	PnlVect* currVect = pnl_vect_create_from_ptr(2, currents);
	LET(currVect, 0) *= GET(currVect, 1) / exp(-interestRate[1] * (maturity-date));

	// On fixe les mu avec les taux sans risque
	PnlVect* trendsVect = pnl_vect_create_from_zero(2);
	LET(trendsVect, 0) = interestRate[0];
	LET(trendsVect, 1) = interestRate[0];


	mod = new BlackScholesModel(2, interestRate[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	double price;
	double ic;
	mc->price(past, date, currVect, &price, &ic);

	PnlVect* deltasQuanto = pnl_vect_create_from_zero(2);
	mc->deltas(past, date, currVect, deltasQuanto);

	double* myDeltasAssets = new double[1];
	double* myDeltasFXRates = new double[1];

	myDeltasAssets[0] = GET(deltasQuanto, 0);
	myDeltasFXRates[0] = GET(deltasQuanto, 1);

	std::cout << "Prix simule : " << price << std::endl;
	std::cout << "ic : " << ic << std::endl;

	*deltasAssets = static_cast<double*>(malloc(1 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 1 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(1 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 1 * sizeof(double));
}
#pragma endregion
#pragma endregion

#pragma region Multimonde 2021 Quanto
#pragma region Inits
/*
 * Initialise le MonteCarlo, l'Option, le BlackScholesModel, en calculant
 * au préalable les nouvelles volatilités, tendances & corrélations.
 */
void InitMultimonde2021Quanto(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	int sampleNumber, // Nombre de tirages ; les taux de changes attendus sont UN EURO EN LA MONNAIE ETRANGERE. (€/$)
	double currentPrices[], // Taille 11. Prix des actifs dans leurs monnaies / taux de change. Indice 0/6 = domestique. 
	double volatilities[], // Taille 11 Volatilités des actifs/tdc dans leurs monnaies ; volatilités des taux de change. Indice 0/6 = domestique.
	double interestRates[], // Taille 6. Taux d'intérêts. Indice 0 = domestique.
	double correlations[]) // Taille 11x11. Corrélations entre les actifs dans leurs monnaies et les tdc €/"$".
{
	// Les actifs manipulés sont les actifs en € (actif "$" * tdc $/€) et les zéros coupons étrangers (tdc $/€ * e^-r$(T-t) )
	// Le début de cette fonction construit tous les paramètres pour cette traduction


	// Calcul de la volatilités des actifs en euros
	// [1-5] actifs étrangers : (vol i = vol actif $ ; vol 5+i = vol €/$ = vol $/€ )
	// vol actif en € = actif en $ / (€/$) => Vol A-B dans l'exponentielle
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(11,volatilities);
	for (int i = 1; i <= 5; i++) {
		LET(volatilitiesVect, i) = VolAminusB(correlations[11 * (i)+(i + 5)], volatilities[i], volatilities[i + 5]);
		// ancien calcul : sqrt(volatilities[5+i] * volatilities[5+i] + volatilities[i] * volatilities[i] - 2 * correlations[11*(i) + (5+i)] * volatilities[i] * volatilities[5+i]);
	}
	// [6-10] zéros coupons : vol ( zéro coupon ) = vol ( tdc $/€ ) = vol ( €/$ ), car dans l'exponentielle. Donc pas de modifications.


	// Calcul des spots des actifs en euros
	PnlVect* spotsVect = pnl_vect_create_from_zero(11);
	// Pour des appels au temps t, les spots sont extraits de past.
	// Le vecteur passé à BlackScholes est donc inutilisé et est purement là pour la compatibilité avec les appels en temps 0 des autres options.
	// Les prix actuels sont passés dans les paramètres de price.


	// Mises des tendances des actifs au taux sans risque domestique ; calcul des tendances des taux de change
	PnlVect* trendsVect = pnl_vect_create(11);
	for (int i = 0; i <= 5; i++) {
		// trends(actifs mis en euros) = r€
		LET(trendsVect, i) = interestRates[0];
	}
	for (int i = 6; i <= 10; i++) {
		LET(trendsVect, i) = interestRates[0];
		// trends(zéro coupon étranger mis en euros) = r€
	}
	//LET(trendsVect, i) = interestRates[i-5] - interestRates[0] + volatilities[i-5]*volatilities[i-5];
	// Anciennement on avait : trends(tdc €/$) = r$ - r€ + sigma$^2


	// Mises à jour des corrélations
	
	// Changements par rapport à (S $/€ ; €/$ ) : les €/$ sont remplacés par $/€ (* f(t), n'importe pas)
	// Par conséquent les taux de change ont toujours même volatilités mais la corrélation opposée avec autrui
	// Les lignes modifiées à l'occasion de la transition au calcul en ZC sont marquées par //ZC
	PnlMat* correlationsMat = pnl_mat_create_from_zero(11, 11);
	for (int y = 0; y <= 10; y++) {
		for (int x = 0; x <= 10; x++) {
			if (x == y) {
				MLET(correlationsMat, y, x) = 1;
			}
			else {
				if (y == 0) { // Actif européen
					if (x <= 5) { // Actif non européen
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (x)+(x + 5)],
							correlations[11 * (x)+(y)],
							correlations[11 * (x + 5) + (y)],
							volatilities[x],
							volatilities[x + 5]);
						//A = actif non européen ; B = taux de change €/$ ; C = actif européen
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = - correlations[11 * (x)+(y)]; //ZC
					}
				} 
				else if (y <= 5) { // Actif non européen
					if (x == 0) { // Actif européen
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (y)+(y + 5)],
							correlations[11 * (y)+(x)],
							correlations[11 * (y + 5) + (x)],
							volatilities[y],
							volatilities[y + 5]);
						//A = actif non européen ; B = taux de change €/$ ; C = actif européen
					}
					else if (x <= 5) { // Actif non européen
						MLET(correlationsMat, y, x) = CorrAminusBwithCminusD(
							correlations[11 * (y)+(y + 5)],
							correlations[11 * (y)+(x)],
							correlations[11 * (y)+(x + 5)],
							correlations[11 * (y + 5) + (x)],
							correlations[11 * (y + 5) + (x + 5)],
							correlations[11 * (x)+(x + 5)],
							volatilities[y],
							volatilities[y + 5],
							volatilities[x],
							volatilities[x + 5]
						);
						//A = actif non européen ; B = taux de change €/$ ; C = actif non européen ; D = taux de change €/$
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (y)+(y + 5)],
							- correlations[11 * (y)+(x)], //ZC
							- correlations[11 * (y + 5) + (x)], //ZC
							volatilities[y],
							volatilities[y + 5]);
						//A = actif non européen ; B = taux de change €/$ ; C = taux de change €/$
					}
				}
				else { // Taux de change
					if (x == 0) { // Actif européen
						MLET(correlationsMat, y, x) = - correlations[11 * (y)+(x)]; //ZC

					}
					else if (x <= 5) { // Actif non européen
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (x)+(x + 5)],
							- correlations[11 * (x)+(y)], //ZC
							- correlations[11 * (x + 5) + (y)], //ZC
							volatilities[x],
							volatilities[x + 5]);
						//A = actif non européen ; B = taux de change €/$ ; C = taux de change €/$
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = correlations[11 * (y)+(x)]; //pas ZC, car - * - = +
					}
				}
			}
		}
	}

	*opt = new Multimonde2021Quanto(interestRates);

	*mod = new BlackScholesModel(11, interestRates[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}

PnlMat* Multimonde2021Quanto_BuildFromPast(
	int nbRows,
	double past[],
	double* interestRates,
	double T,
	PnlVect* dates) {
	// Mises des actifs dans leurs monnaies à l'aide du TDC €/$
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 11, past);
	for (int y = 0; y < nbRows; y++) {
		for (int x = 1; x <= 5; x++) {
			MLET(pastMat, y, x) /= MGET(pastMat, y, x + 5);
		}
	}
	// Remplacement du TDC €/$ par la valeur du zéro-coupon ($/€ actualisé)
	for (int y = 0; y < nbRows; y++) {
		for (int x = 6; x <= 10; x++) {
			MLET(pastMat, y, x) = 1/MGET(pastMat, y, x) * exp(-interestRates[x - 5] * (T - GET(dates, y)));
		}
	}
	return pastMat;
}
PnlVect* Multimonde2021Quanto_BuildFromCurrentPrices(
	double current[],
	double* interestRates,
	double t,
	double T
) {
	PnlVect* toBeReturned = pnl_vect_create_from_ptr(11, current);
	for (int i = 1; i <= 5; i++) {
		LET(toBeReturned, i) /= GET(toBeReturned, i + 5);
	}
	// Remplacement du TDC €/$ par la valeur du zéro-coupon
	for (int x = 6; x <= 10; x++) {
		LET(toBeReturned, x) = 1/GET(toBeReturned, x) * exp(-interestRates[x - 5] * (T -t));
	}
	return toBeReturned;
}

#pragma endregion
#pragma region Price
void PriceMultimonde2021Quanto(
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[], //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021Quanto(&mc, &opt, &mod, sampleNumber, currentPrices, volatilities, interestRates, correlations);

	//Gestions paramètres past
	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past, interestRates, opt->T, opt->customDates);
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices, interestRates, t, opt->T);

	mc->price(pastMat, t, currentVect, price, ic);
}
#pragma endregion
#pragma region Deltas
void DeltasMultimonde2021Quanto(
	int sampleNumber,
	double past[], // format [,]
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	double** deltas)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitMultimonde2021Quanto(&mc, &opt, &mod, sampleNumber, currentPrices, volatilities, interestRates, correlations);

	//Gestions paramètres past
	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past, interestRates, opt->T, opt->customDates);
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices, interestRates, t, opt->T);

	PnlVect* myDeltas = pnl_vect_create_from_zero(11);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	// Les 'actifs' manipulés par le modèle sont "[actif en [$]] [$]/€" et "€/[$]"
	// L'appel doit renvoyer des données directement utilisables pour la couverture, ici
	// d([$]/€) et d([nombre d'actif])

	double* deltasIntermediate = new double[11];
	for (int i = 0; i <= 10; i++) {
		deltasIntermediate[i] = GET(myDeltas, i);
	}
	*deltas = static_cast<double*>(malloc(11 * sizeof(double)));
	memcpy(*deltas, &(deltasIntermediate[0]), 11 * sizeof(double));
}
#pragma endregion
#pragma region Tracking error
void TrackingErrorMultimonde2021Quanto(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	int nbUpdates,
	double* tracking_error,
	double** portfolioReturns,
	double ** productReturns) {

	//pour l'instant, t est ignoré
	//std::cout << "Tracking error > 1" << std::endl;
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	//std::cout << "Tracking error > 2" << std::endl;

	InitMultimonde2021Quanto(&mc, &opt, &mod, sampleNumber, currentPrices, volatilities, interestRates, correlations);
	mc->nbSamples_ = sampleNumber; //à appeler quand deltas
	//std::cout << "Tracking error > 3" << std::endl;

	//dates
	PnlVect* dates = pnl_vect_create(nbUpdates + 1);
	for (int i = 0; i < nbUpdates + 1; i++) {
		LET(dates, i) = i / 365.25;
	}

	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past, interestRates, opt->T, dates); // Ici on passe dates car le format de past est celui de scénario
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices, interestRates, t, opt->T);

	PnlMat *scenario = pnl_mat_create(371*6 + 1, mod->size_);
	//std::cout << "Tracking error > 4" << std::endl;

	mod->initAsset(nbUpdates);
	
	//std::cout << "Tracking error > 5" << std::endl;

	mod->postInitAssetCustomDates(scenario,
		pastMat, t, currentVect,
		dates, nbUpdates, mc->rng_);
	//std::cout << "Tracking error > 6" << std::endl;

	double portfolioReturn;
	double previousValue;
	double value;
	double spare = 0;
	PnlVect* quantities = pnl_vect_create_from_zero(11);

	int advancement = 0;
	double productReturn;
	double previousPrice;
	double price;
	double ic;
	 
	PnlVect* returnsDiff = pnl_vect_create(nbUpdates);
	double* portfolioReturnsIntermediate = new double[nbUpdates];
	double* productReturnsIntermediate = new double[nbUpdates];


	mc->price(scenario, GET(dates,advancement), currentVect, &price, &ic); // le problème est ici
	PnlVect* deltas = pnl_vect_create_from_zero(11);
	mc->deltasMultimonde2021Quanto(scenario, GET(dates, advancement), currentVect, deltas);
	spare = price;
	UpdatePortfolio(quantities, currentVect, deltas, spare);
	value = ComputeValue(quantities, currentVect) + spare;
	bool verbose = false;
	bool stepByStep = false;
	double step = (371 * 6 / 365.25) / nbUpdates;
	//pnl_mat_print(scenario);
	for (int advancement = 1; advancement<nbUpdates; advancement++) {
		// mise à jour des informations
		std::cout << "Advancement : " << advancement << std::endl;
		if (verbose) std::cout << "Step : " << step << std::endl;
		if (stepByStep && advancement>5) std::cin.ignore();
		pnl_mat_get_row(currentVect, scenario, advancement);
		if (verbose) std::cout << "Current prices : " << std::endl; if (verbose) pnl_vect_print(currentVect);
		// calcul du rendement du portefeuille
		if (verbose) std::cout << "Current quantities : " << std::endl; if (verbose) pnl_vect_print(quantities);
		if (verbose) std::cout << "Spare : " << spare << std::endl;
		if (stepByStep && advancement>5) std::cin.ignore();
		UpdateCurrencyQuantities(step, &spare, 6, quantities, interestRates);
		if (verbose) std::cout << "After actualisation : " << std::endl; if (verbose) pnl_vect_print(quantities);
		if (verbose) std::cout << "Spare : " << spare << std::endl;
		previousValue = value;
		if (verbose) std::cout << "Previous value : " << previousValue << std::endl;
		if (stepByStep && advancement>5) std::cin.ignore();
		value = ComputeValue(quantities, currentVect) + spare;
		if (verbose) std::cout << "New value : " << value << std::endl;
		portfolioReturnsIntermediate[advancement] = portfolioReturn = value / previousValue;
		if (verbose) std::cout << "Portfolio return : " << portfolioReturn << std::endl;
		// calcul du rendement du multimonde
		previousPrice = price;
		if (verbose) std::cout << "Previous price : " << previousPrice << std::endl;
		if (stepByStep && advancement>5) std::cin.ignore();
		mc->price(scenario, GET(dates, advancement), currentVect, &price, &ic);
		if (verbose) std::cout << "New price : " << price << std::endl;
		productReturnsIntermediate[advancement] = productReturn = price / previousPrice;
		if (verbose) std::cout << "Product return : " << productReturn << std::endl;
		// calcul de la différence
		if (stepByStep && advancement>5) std::cin.ignore();
		LET(returnsDiff, advancement - 1) = portfolioReturn - productReturn;
		if (verbose) std::cout << "Return difference : " << GET(returnsDiff, advancement - 1) << std::endl;
		// mise à jour de la composition du portefeuille
		if (stepByStep && advancement>5) std::cin.ignore();
		mc->nbSamples_ /= 10;
		mc->deltasMultimonde2021Quanto(scenario, GET(dates, advancement), currentVect, deltas);
		mc->nbSamples_ *= 10;
		if (verbose) std::cout << "Deltas : " << std::endl; if (verbose) pnl_vect_print(deltas);

		// Pour calculer la tracking error, on ne simule pas une stratégie de couverture - on regarde juste à chaque date
		// la performance de la couverture jusqu'à la suivante. On considère donc à chaque fois "magiquement" qu'on a à
		// nouveau le prix comme quantité d'€ disponible. Artificiel mais plus propre calculatoirement.
		// Repasser à une courbe ne devrait alors pas être trop difficile (prix initial + multiplication par rendements )

		if (stepByStep && advancement>5) std::cin.ignore();
		PnlVect* quantities = pnl_vect_create_from_zero(11);
		spare = price;
		UpdatePortfolio(quantities, currentVect, deltas, spare);

		if (verbose) std::cout << "New quantities : " << std::endl; if (verbose) pnl_vect_print(quantities);
		if (verbose) std::cout << "Spare : " << spare << std::endl;
		if (stepByStep) std::cin.ignore();

		//if (verbose) std::cout << "; " << value << " ; " << ComputeValue(quantities, currentVect) + spare << std::endl;
		//std::cin.ignore();
	}
	// calcul de la tracking error
	double sum = 0;
	double squaresSum = 0;
	pnl_vect_print(returnsDiff);
	for (int i = 0; i < nbUpdates; i++) {
		sum += GET(returnsDiff, i);
		squaresSum += GET(returnsDiff, i)*GET(returnsDiff, i);
	}
	std::cout << squaresSum << std::endl;
	std::cout << sum << std::endl;
	std::cout << sqrt(squaresSum - sum*sum) << std::endl;

	*productReturns = static_cast<double*>(malloc(nbUpdates * sizeof(double)));
	memcpy(*productReturns, &(productReturnsIntermediate[0]), nbUpdates * sizeof(double));

	*portfolioReturns = static_cast<double*>(malloc(nbUpdates * sizeof(double)));
	memcpy(*portfolioReturns, &(portfolioReturnsIntermediate[0]), nbUpdates * sizeof(double));

	*tracking_error = sqrt(squaresSum - sum*sum);
}
#pragma endregion
#pragma endregion

#pragma region SingleMonde
#pragma region Inits
/*
* Initialise le MonteCarlo, l'Option, le BlackScholesModel, en calculant
* au préalable les nouvelles volatilités, tendances & corrélations.
*/
void InitSingleMonde(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	int sampleNumber, // Nombre de tirages ; les taux de changes attendus sont UN EURO EN LA MONNAIE ETRANGERE. (€/$)
	double currentPrices[], // Taille 2. Prix de l'actif dans sa monnaie / taux de change €/$.
	double volatilities[], // Taille 2 Volatilités de l'actif dans sa monnaie / du taux de change €/$.
	double interestRates[], // Taille 2. Taux d'intérêts. Indice 0 = domestique.
	double correlations[]) // Taille 2x2. Corrélations entre les actifs dans leurs monnaies et le le tdc €/$.
{
	// Calcul de la volatilité
	// vol actif en € = actif en $ / (€/$) => Vol A-B dans l'exponentielle
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(11, volatilities);
	LET(volatilitiesVect, 0) = VolAminusB(correlations[1], volatilities[0], volatilities[1]);

	// Calcul des spots des actifs en euros
	PnlVect* spotsVect = pnl_vect_create_from_zero(11);
	// Voir commentaire spotsVect Multimonde2021Q

	// Mises des tendances des actifs au taux sans risque domestique ; calcul des tendances des taux de change
	PnlVect* trendsVect = pnl_vect_create(11);
	LET(trendsVect, 0) = interestRates[0];
	LET(trendsVect, 1) = interestRates[0];

	// Mises à jour des corrélations
	PnlMat* correlationsMat = GenCorrAMinusBBFromCorrAB(correlations, volatilities);

	*opt = new Multimonde2021Quanto(interestRates);

	*mod = new BlackScholesModel(11, interestRates[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}

PnlMat* SingleMonde_BuildFromPast(
	int nbRows,
	double past[],
	double* interestRates,
	double T,
	PnlVect* dates) {
	// Mises des actifs dans leurs monnaies à l'aide du TDC €/$
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 11, past);
	for (int y = 0; y < nbRows; y++) {
		for (int x = 1; x <= 5; x++) {
			MLET(pastMat, y, x) /= MGET(pastMat, y, x + 5);
		}
	}
	// Remplacement du TDC €/$ par la valeur du zéro-coupon
	for (int y = 0; y < nbRows; y++) {
		for (int x = 6; x <= 10; x++) {
			MLET(pastMat, y, x) = 1/MGET(pastMat, y, x) * exp(-interestRates[x - 5] * (T - GET(dates, y)));
		}
	}
	return pastMat;
}
PnlVect* SingleMonde_BuildFromCurrentPrices(
	double current[], // actif en $ ; un € en $
	double* interestRates,
	double t,
	double T
) {
	PnlVect* toBeReturned = pnl_vect_create_from_ptr(11, current);
	LET(toBeReturned, 0) /= GET(toBeReturned, 1);
	// Remplacement du TDC €/$ par la valeur du zéro-coupon
	LET(toBeReturned, 1) = 1/GET(toBeReturned, 1) * exp(-interestRates[1] * (T - t));

	return toBeReturned;
}
#pragma endregion
void PriceSingleMonde(
	int sampleNumber,
	double past[],
	int nbRows,
	double t,
	double currentPrices[],
	double volatilities[],
	double interestRates[],
	double correlations[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	InitSingleMonde(&mc, &opt, &mod, sampleNumber, currentPrices, volatilities, interestRates, correlations);

	//Gestions paramètres past
	PnlMat* pastMat = SingleMonde_BuildFromPast(nbRows, past, interestRates, opt->T, opt->customDates);
	PnlVect* currentVect = SingleMonde_BuildFromCurrentPrices(currentPrices, interestRates, t, opt->T);

	mc->price(pastMat, t, currentVect, price, ic);
}
#pragma endregion

#pragma region Utils

double call_pnl_cdfnor(double x) {
	return pnl_cdfnor(x);
}
#pragma endregion