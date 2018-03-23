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
	double correlation[], //optionSize�, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
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
	double correlation[], //optionSize�, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasketAnyTime(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestion param�tres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra �tre multidimensionnel [,], pas jagged !
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

	//Gestion param�tres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra �tre multidimensionnel [,], pas jagged !
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

	//Gestion param�tres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra �tre multidimensionnel [,], pas jagged !
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

	//Gestions param�tres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c# devra �tre multidimensionnel [,], pas jagged !
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
	* Pour obtenir le delta en "0" il faut shifter en "0+". Math�matiquement, on obtiendrait un delta
	* ne rendant pas compte du changement de performance mais juste de l'�ventualit� qu'on se heurte au mur des 15%.
	* C'est le cas pour toutes les dates de constation, on s'int�resse � l'instant suivant.
	* Exemple, avec une option qui donne la performance de l'option entre 0 et T (ie prix de l'actif en T / prix
	* de l'actif en 0), si on shift en 0 et non 0+, alors le delta obtenu pour cet actif par rapport � son sous-jacent
	* serait de 0 alors qu'il est �videmment de 1. - Julien
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

	//Gestions param�tres past
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

	//Gestions param�tres past
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
	double* price,
	double* ic)
{

	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	opt = new QuantoOption(maturity, strike);

	// LE POINT QUI CLOCHE EST ICI JE PENSE [yoann]

	// Calcul de la vol de S-X
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(2, volatilities);
	LET(volatilitiesVect, 0) = sqrt(volatilities[1] * volatilities[1] + volatilities[0] * volatilities[0] - 2 * correlations[1] * volatilities[0] * volatilities[1]);
	std::cout << "Volatilities" << std::endl;
	pnl_vect_print_asrow(volatilitiesVect);

	// Calcul de la cor de S-X et X
	PnlMat* correlationsMat = GenCorrAPlusBBFromCorrAB(correlations, volatilities);
	std::cout << "Correlations" << std::endl;

	pnl_mat_print(correlationsMat);

	// On actualise le prix en euros
	PnlVect* spotsVect = pnl_vect_create_from_ptr(2, spots);
	LET(spotsVect, 0) /= GET(spotsVect, 1);
	std::cout << "Spots en euros" << std::endl;
	pnl_vect_print_asrow(spotsVect);

	// On fixe les mu avec les taux sans risque
	PnlVect* trendsVect = pnl_vect_create_from_zero(2);
	LET(trendsVect, 0) = interestRate[0];
	LET(trendsVect, 1) = interestRate[1] - interestRate[0] + volatilities[1] * volatilities[1];
	std::cout << "Trends de la simulation" << std::endl;
	pnl_vect_print_asrow(trendsVect);

	mod = new BlackScholesModel(2, interestRate[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));

	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	mc->price(price, ic);
}
#pragma endregion
#pragma simulDeltas
void SimulDeltasQuanto(
	double maturity,
	double strike,
	int sampleNumber,
	double spots[],
	double volatilities[],
	double interestRate[],
	double correlations[],
	double** deltasAssets,
	double** deltasFXRates)
{

	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;

	opt = new QuantoOption(maturity, strike);


	// Calcul de la vol de S-X
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(2, volatilities);
	LET(volatilitiesVect, 0) = sqrt(volatilities[1] * volatilities[1] + volatilities[0] * volatilities[0] - 2 * correlations[1] * volatilities[0] * volatilities[1]);
	LET(volatilitiesVect, 1) *= -1;

	// Calcul de la cor de S-X et X
	PnlMat* correlationsMat = GenCorrAPlusBBFromCorrAB(correlations, volatilities);


	// On actualise le prix en euros
	PnlVect* spotsVect = pnl_vect_create_from_ptr(2, spots);
	//LET(spotsVect, 0) /= GET(spotsVect, 1);
	LET(spotsVect, 0) *= GET(spotsVect, 1) / exp(-interestRate[1] * maturity);

	// On fixe les mu avec les taux sans risque
	PnlVect* trendsVect = pnl_vect_create_from_zero(2);
	LET(trendsVect, 0) = interestRate[0];
	//LET(trendsVect, 1) = interestRate[1] - interestRate[0] +volatilities[1] * volatilities[1];
	LET(trendsVect, 1) = interestRate[0];


	mod = new BlackScholesModel(2, interestRate[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));


	mc = new MonteCarlo(mod, opt, rng, sampleNumber);

	PnlVect* deltasQuanto = pnl_vect_create_from_zero(2);
	mc->deltas(deltasQuanto);

	//std::cout << "deltasQuanto apres appel a mc->deltas:" << std::endl;
	//pnl_vect_print(deltasQuanto);

	double* myDeltasAssets = new double[1];
	double* myDeltasFXRates = new double[1];

	myDeltasAssets[0] = GET(deltasQuanto, 0);
	myDeltasFXRates[0] = GET(deltasQuanto, 1);// *GET(spotsVect, 1);// / (exp(-interestRate[1] * maturity));

	double price;
	double ic;
	mc->price(&price, &ic);
	std::cout << "Prix simule : " << price << std::endl;
	std::cout << "ic : " << ic << std::endl;

	*deltasAssets = static_cast<double*>(malloc(2 * sizeof(double)));
	memcpy(*deltasAssets, &(myDeltasAssets[0]), 2 * sizeof(double));

	*deltasFXRates = static_cast<double*>(malloc(2 * sizeof(double)));
	memcpy(*deltasFXRates, &(myDeltasFXRates[0]), 2 * sizeof(double));
}
#pragma endregion
#pragma endregion

#pragma region Multimonde 2021 Quanto
#pragma region Inits
/*
 * Initialise le MonteCarlo, l'Option, le BlackScholesModel, en calculant
 * au pr�alable les nouvelles volatilit�s, tendances & corr�lations.
 */
void InitMultimonde2021Quanto(
	MonteCarlo** mc,
	Option** opt,
	BlackScholesModel** mod,
	int sampleNumber, // Nombre de tirages ; les taux de changes attendus sont UN EURO EN LA MONNAIE ETRANGERE. (�/$)
	double currentPrices[], // Taille 11. Prix des actifs dans leurs monnaies / taux de change. Indice 0/6 = domestique. 
	double volatilities[], // Taille 11 Volatilit�s des actifs/tdc dans leurs monnaies ; volatilit�s des taux de change. Indice 0/6 = domestique.
	double interestRates[], // Taille 6. Taux d'int�r�ts. Indice 0 = domestique.
	double correlations[]) // Taille 11x11. Corr�lations entre les actifs dans leurs monnaies et les tdc �/"$".
{

	// Calcul de la volatilit�s des actifs en euros
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(11,volatilities);
	for (int i = 1; i <= 5; i++) {
		LET(volatilitiesVect, i) = sqrt(volatilities[5+i] * volatilities[5+i] + volatilities[i] * volatilities[i] - 2 * correlations[11*(i) + (5+i)] * volatilities[i] * volatilities[5+i]);
	}

	// Calcul des spots des actifs en euros
	PnlVect* spotsVect = pnl_vect_create_from_zero(11);
	// Pour des appels au temps t, les spots sont extraits de past.
	// Le vecteur pass� � BlackScholes est donc inutilis� et est purement l� pour la compatibilit� avec les appels en temps 0 des autres options.

	/*PnlVect* spotsVect = pnl_vect_create_from_ptr(11, currentPrices);
	for (int i = 1; i <= 5; i++) {
		LET(spotsVect, i) /= GET(spotsVect, 5+i);
	}*/

	// Mises des tendances des actifs au taux sans risque domestique ; calcul des tendances des taux de change
	PnlVect* trendsVect = pnl_vect_create(11);
	for (int i = 0; i <= 5; i++) {
		LET(trendsVect, i) = interestRates[0];
	} // trends(actifs) = r�
	for (int i = 6; i <= 10; i++) {
		LET(trendsVect, i) = interestRates[i-5] - interestRates[0] + volatilities[i-5]*volatilities[i-5];
	} // trends(tdc �/$) = r$ - r� + sigma$^2

	// Mises � jour des corr�lations
	PnlMat* correlationsMat = pnl_mat_create_from_zero(11, 11);
	for (int y = 0; y <= 10; y++) {
		for (int x = 0; x <= 10; x++) {
			if (x == y) {
				MLET(correlationsMat, y, x) = 1;
			}
			else {
				if (y == 0) { // Actif europ�en
					if (x <= 5) { // Actif non europ�en
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (x)+(x + 5)],
							correlations[11 * (x)+(y)],
							correlations[11 * (x + 5) + (y)],
							volatilities[x],
							volatilities[x + 5]);
						//A = actif non europ�en ; B = taux de change �/$ ; C = actif europ�en
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = correlations[11 * (x)+(y)];
					}
				} 
				else if (y <= 5) { // Actif non europ�en
					if (x == 0) { // Actif europ�en
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (y)+(y + 5)],
							correlations[11 * (y)+(x)],
							correlations[11 * (y + 5) + (x)],
							volatilities[y],
							volatilities[y + 5]);
						//A = actif non europ�en ; B = taux de change �/$ ; C = actif europ�en
					}
					else if (x <= 5) { // Actif non europ�en
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
						//A = actif non europ�en ; B = taux de change �/$ ; C = actif non europ�en ; D = taux de change �/$
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (y)+(y + 5)],
							correlations[11 * (y)+(x)],
							correlations[11 * (y + 5) + (x)],
							volatilities[y],
							volatilities[y + 5]);
						//A = actif non europ�en ; B = taux de change �/$ ; C = taux de change �/$
					}
				}
				else { // Taux de change
					if (x == 0) { // Actif europ�en
						MLET(correlationsMat, y, x) = correlations[11 * (y)+(x)];

					}
					else if (x <= 5) { // Actif non europ�en
						MLET(correlationsMat, y, x) = CorrAminusBwithC(
							correlations[11 * (x)+(x + 5)],
							correlations[11 * (x)+(y)],
							correlations[11 * (x + 5) + (y)],
							volatilities[x],
							volatilities[x + 5]);
						//A = actif non europ�en ; B = taux de change �/$ ; C = taux de change �/$
					}
					else { // Taux de change
						MLET(correlationsMat, y, x) = correlations[11 * (y)+(x)];
					}
				}
			}
		}
	}

	*opt = new Multimonde2021Quanto();

	*mod = new BlackScholesModel(11, interestRates[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);

}

PnlMat* Multimonde2021Quanto_BuildFromPast(
	int nbRows,
	double past[]
) {
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 11, past);
	for (int y = 0; y < nbRows; y++) {
		for (int x = 1; x <= 5; x++) {
			MLET(pastMat, y, x) /= MGET(pastMat, y, x + 5);
		}
	}
	return pastMat;
}
PnlVect* Multimonde2021Quanto_BuildFromCurrentPrices(double current[]) {
	PnlVect* toBeReturned = pnl_vect_create_from_ptr(11, current);
	for (int i = 1; i <= 5; i++) {
		LET(toBeReturned, i) /= GET(toBeReturned, i + 5);
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

	//Gestions param�tres past
	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past);
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices);

	mc->price(pastMat, t, currentVect, price, ic);

}
#pragma endregion
#pragma region Deltas
void DeltasMultimonde2021Quanto (
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


	//Gestions param�tres past
	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past);
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices);

	PnlVect* myDeltas = pnl_vect_create_from_zero(11);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	// Les 'actifs' manipul�s par le mod�le sont "[actif en [$]] [$]/�" et "�/[$]"
	// L'appel doit renvoyer des donn�es directement utilisables pour la couverture, ici
	// d([$]/�) et d([nombre d'actif])

	double* deltasIntermediate = new double[11];
	deltasIntermediate[0] = GET(myDeltas, 0);
	// Correspond aux calculs d'Alexandra (+ facteur x � dt)
	for (int i = 6; i <= 10; i++) {
		deltasIntermediate[i] = - currentPrices[i] * currentPrices[i] * GET(myDeltas, i)
			+ currentPrices[i] * volatilities[i]*volatilities[i]/2.0;
	}
	// Correspond aux calculs d'Alexandra
	for (int i = 1; i <= 5; i++) {
		deltasIntermediate[i] = -1.0 / currentPrices[i + 5] * (
			GET(myDeltas, i)
			- currentPrices[i] * deltasIntermediate[i + 5]
			- (1.0 / 2.0 * currentPrices[i + 5] * volatilities[i + 5] * volatilities[i + 5]
				+ 1.0 / 2.0 * currentPrices[i] * volatilities[i] * volatilities[i]
				+ correlations[11 * (i)+(i + 5)] * currentPrices[i + 5] * volatilities[i] * volatilities[i + 5])
			);
	}

	*deltas = static_cast<double*>(malloc(11 * sizeof(double)));
	memcpy(*deltas, &(deltasIntermediate[0]), 11 * sizeof(double));
}
void DeltasMultimonde2021QuantoDebug(
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

	//Gestions param�tres past
	PnlMat* pastMat = Multimonde2021Quanto_BuildFromPast(nbRows, past);
	PnlVect* currentVect = Multimonde2021Quanto_BuildFromCurrentPrices(currentPrices);

	PnlVect* myDeltas = pnl_vect_create_from_zero(11);
	mc->deltas(pastMat, t, currentVect, myDeltas);

	// Les 'actifs' manipul�s par le mod�le sont "[actif en [$]] [$]/�" et "�/[$]"
	// L'appel doit renvoyer des donn�es directement utilisables pour la couverture, ici
	// d([$]/�) et d([nombre d'actif])

	double* deltasIntermediate = new double[11];
	for (int i = 0; i <= 10; i++) {
		deltasIntermediate[i] = GET(myDeltas, i);
		std::cout << deltasIntermediate[i] << std::endl;
	}
	*deltas = static_cast<double*>(malloc(11 * sizeof(double)));
	memcpy(*deltas, &(deltasIntermediate[0]), 11 * sizeof(double));
}

#pragma region SingleMonde
void PriceSingleMonde(int sampleNumber,
	//double past[], // format [,]
	double currentPrices[],//taille 1, il s'agit juste du spot
	double volatilities[],//taille 1 pareil
	double interestRates[],//pour l'instant taille 1
	double* price,
	double T,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	opt = new SingleMonde(T);
	int OptionSize = 1;
	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	PnlMat *corr = pnl_mat_create_from_double(1, 1, 1.0);
	PnlVect* sigma = pnl_vect_create_from_double(1, volatilities[0]);
	PnlVect* spot = pnl_vect_create_from_double(1, currentPrices[0]);
	PnlVect* rate = pnl_vect_create_from_double(1, interestRates[0]);
	mod = new BlackScholesModel(OptionSize, interestRates[0], corr, sigma,spot,rate);
	mc = new MonteCarlo(mod, opt, rng, sampleNumber);
	mc->price(price, ic);

}
#pragma endregion
#pragma endregion
#pragma endregion

#pragma region Utils

double call_pnl_cdfnor(double x) {
	return pnl_cdfnor(x);
}
#pragma endregion