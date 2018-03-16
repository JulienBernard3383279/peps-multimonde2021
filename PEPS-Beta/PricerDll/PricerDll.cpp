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
	double correlation[], //optionSize*optionSize, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
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
	double correlation[], //optionSize*optionSize, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
	double trends[],
	double* price,
	double* ic)
{
	MonteCarlo *mc;
	Option *opt;
	BlackScholesModel *mod;
	InitBasketAnyTime(&mc, &opt, &mod, maturity, optionSize, strike, payoffCoefficients, sampleNumber, volatilities, interestRate, correlation, trends);

	//Gestion paramètres past
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c-sharp devra être multidimensionnel [,], pas jagged !
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
	PnlMat* pastMat = pnl_mat_create_from_ptr(nbRows, 6, past); //Le tableau c-sharp devra être multidimensionnel [,], pas jagged !
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

	*deltas = static_cast<double*>(malloc(6 * sizeof(double)));
	memcpy(*deltas, &(intermediate[0]), 6 * sizeof(double));
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
		myDeltasAssets[i] = GET(deltas, i) / FXRates[i];
		myDeltasFXRates[i] = -GET(deltas, i) * spots[i] / FXRates[i];
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
		myDeltasFXRates[i] = -deltas[i] * prices[i] / FXRates[i];
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
	PnlVect* tmp_current_fx_rates = pnl_vect_create_from_double(opt->size - 1, 1.);
	PnlVect* current_fx_rates = pnl_vect_create_from_double(opt->size - 1, 1.);

	int t = 0;
	int T = 371 * 6;
	double port = 0.;

	pnl_mat_get_row(current_prices, asset_prices, 0);

	pnl_mat_get_row(tmp_current_fx_rates, FX_prices, t);
	current_fx_rates = pnl_vect_create_from_zero(5);
	pnl_vect_clone(current_fx_rates, tmp_current_fx_rates);
	pnl_vect_resize(current_fx_rates, current_fx_rates->size + 1);
	for (int i = current_fx_rates->size - 1; i > 0; i--) {
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

	while (t < T) {
		t += 1;
		pnl_mat_get_row(current_prices, asset_prices, t);

		pnl_mat_get_row(tmp_current_fx_rates, FX_prices, t);
		pnl_vect_free(&current_fx_rates);
		current_fx_rates = pnl_vect_create_from_zero(5);
		pnl_vect_clone(current_fx_rates, tmp_current_fx_rates);
		pnl_vect_resize(current_fx_rates, current_fx_rates->size + 1);
		for (int i = current_fx_rates->size - 1; i > 0; i--) {
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
	double spots[], // Taille 11. Spots des actifs/tdc dans leurs monnaies. Indice 0/6 = domestique. 
	double volatilities[], // Taille 11 Volatilités des actifs/tdc dans leurs monnaies ; volatilités des taux de change. Indice 0/6 = domestique.
	double interestRates[], // Taille 6. Taux d'intérêts. Indice 0 = domestique.
	double correlations[]) // Taille 11x11. Corrélations entre les actifs dans leurs monnaies et les tdc €/"$".
{
	// Calcul de la volatilités des actifs en euros
	PnlVect* volatilitiesVect = pnl_vect_create_from_ptr(11,volatilities);
	for (int i = 1; i <= 5; i++) {
		LET(volatilitiesVect, i) = sqrt(volatilities[5+i] * volatilities[5+i] + volatilities[i] * volatilities[i] - 2 * correlations[11*(i) + (5+i)] * volatilities[i] * volatilities[5+i]);
	}

	// Calcul des spots des actifs en euros
	PnlVect* spotsVect = pnl_vect_create_from_ptr(11, spots);
	for (int i = 1; i <= 5; i++) {
		LET(spotsVect, i) /= GET(spotsVect, 5+i);
	}

	// Mises des tendances des actifs au taux sans risque domestique ; calcul des tendances des taux de change
	PnlVect* trendsVect = pnl_vect_create(11);
	for (int i = 0; i <= 5; i++) {
		LET(trendsVect, i) = interestRates[0];
	} // trends(actifs) = r€
	for (int i = 6; i <= 10; i++) {
		LET(trendsVect, i) = interestRates[i-5] - interestRates[0] + volatilities[i-5]*volatilities[i-5];
	} // trends(tdc €/$) = r$ - r€ + sigma$^2
	
	// Mises à jour des corrélations
	PnlMat* correlationsMat = pnl_mat_create_from_zero(11, 11);
	
	//actifs-tdc //WIP
	//actifs-actifs //WIP

	*opt = new Multimonde2021Quanto();

	*mod = new BlackScholesModel(11, interestRates[0], correlationsMat, volatilitiesVect, spotsVect, trendsVect);

	PnlRng *rng = pnl_rng_create(0);
	pnl_rng_sseed(rng, time(NULL));
	*mc = new MonteCarlo(*mod, *opt, rng, sampleNumber);
}
#pragma endregion
#pragma endregion

#pragma region Utils
double call_pnl_cdfnor(double x) {
	return pnl_cdfnor(x);
}
#pragma endregion