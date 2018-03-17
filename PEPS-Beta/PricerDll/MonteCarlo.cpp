#pragma once

#include "stdafx.h"

#include <iostream>
#include <cmath>

#include "MonteCarlo.h"
#include "Multimonde2021.h"
using namespace std;

MonteCarlo::MonteCarlo() {}

MonteCarlo::MonteCarlo(BlackScholesModel *mod, Option *opt, PnlRng *rng, int nbSamples) {
	mod_ = mod;
	opt_ = opt;
	rng_ = rng;
	nbSamples_ = nbSamples;
}

MonteCarlo::~MonteCarlo() {}

void MonteCarlo::price(double* prix, double* ic) {

	double mySum = 0;
	double mySquaredSum = 0;
	double theirSum = 0;
	double theirSquaredSum = 0;
	double var = 0;

	PnlMat *path = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);

	mod_->initAsset(opt_->nbTimeSteps);
	for (int i = 0; i < nbSamples_; ++i) {
		if (! opt_->custom) { mod_->postInitAsset(path, opt_->T, opt_->nbTimeSteps, rng_); }
		else { mod_->postInitAssetCustomDates(path, opt_->customDates, opt_->nbTimeSteps, rng_); }

		double payoff = opt_->payoff(path);
		if (i % 1000 == 0 || i == nbSamples_ -1) {
			//std::cout << payoff << std::endl;
		}
		mySum += payoff;
		mySquaredSum += payoff * payoff;
	}

	*prix = mySum / nbSamples_ * exp(- mod_->r_ * opt_->T);

	var = exp(-mod_->r_ * opt_->T * 2)
		* (mySquaredSum / nbSamples_ - pow(mySum / nbSamples_, 2));

	*ic = 2 * 1.96 * sqrt(var) / sqrt(nbSamples_);

	// Free memory
	pnl_mat_free(&path);
}

void MonteCarlo::deltas(PnlVect* deltas) {
	PnlMat *path = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	PnlMat *pathMinus = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	PnlMat *pathPlus = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);

	double payoffMinus = 0;
	double payoffPlus = 0;

	mod_->initAsset(opt_->nbTimeSteps);
	for (int i = 0; i < nbSamples_; ++i) {
		if (!opt_->custom) { mod_->postInitAsset(path, opt_->T, opt_->nbTimeSteps, rng_); }
		else { mod_->postInitAssetCustomDates(path, opt_->customDates, opt_->nbTimeSteps, rng_); }
		
		for (int j = 0; j < mod_->size_; j++) {
			mod_->shiftPath(path, pathMinus, pathPlus, j, 1, opt_->nbTimeSteps, 0.01);
			payoffPlus = opt_->payoff(pathPlus);
			payoffMinus = opt_->payoff(pathMinus);
			
			
			LET(deltas,j) += (payoffPlus - payoffMinus) / (MGET(path, 0, j) * 2 * 0.01);
		}
	}

	for (int j = 0; j < mod_->size_; j++) {
		LET(deltas,j) /= nbSamples_;
		LET(deltas,j) *= exp(-mod_->r_ * opt_->T);
	}

	return;
}

void MonteCarlo::price(PnlMat* past, double t, PnlVect* current, double* prix, double* ic) {

	double mySum = 0;
	double mySquaredSum = 0;
	double theirSum = 0;
	double theirSquaredSum = 0;
	double var = 0;

	PnlMat *path = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	mod_->initAsset(opt_->nbTimeSteps);

	double tempPayoff = 0;

	for (int i = 0; i < nbSamples_; ++i) {

		if (!opt_->custom) {
			mod_->postInitAsset(path,
				past, t, current,
				opt_->T, opt_->nbTimeSteps, rng_);
		}
		else {
			mod_->postInitAssetCustomDates(path,
			                                  past, t, current,
			                                  opt_->customDates, opt_->nbTimeSteps, rng_); }
		tempPayoff = opt_->payoff(path);

		mySum += tempPayoff;
		mySquaredSum += tempPayoff * tempPayoff;
	}

	*prix = mySum / nbSamples_ * exp(-mod_->r_ * (opt_->T - t) );

	var = exp(-mod_->r_ * (opt_->T - t )* 2)
		* (mySquaredSum / nbSamples_ - pow(mySum / nbSamples_, 2));

	*ic = 2 * 1.96 * sqrt(var) / sqrt(nbSamples_);

	// Free memory
	pnl_mat_free(&path);

}

void MonteCarlo::deltas(PnlMat *past, double t, PnlVect* current, PnlVect* deltas) {

	PnlMat *path = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	PnlMat *pathMinus = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	PnlMat *pathPlus = pnl_mat_create(opt_->nbTimeSteps + 1, mod_->size_);
	
	double payoffMinus = 0;
	double payoffPlus = 0;

	mod_->initAsset(opt_->nbTimeSteps);

	for (int i = 0; i < nbSamples_; ++i) {
		if (!opt_->custom) {
			mod_->postInitAsset(path,
				past, t, current,
				opt_->T, opt_->nbTimeSteps, rng_);
		}
		else {
			mod_->postInitAssetCustomDates(path,
				past, t, current,
				opt_->customDates, opt_->nbTimeSteps, rng_);
		}

		int from = 0;
		if (opt_->custom) {

			while (GET(opt_->customDates, from) < t) {
				from++;
			}
		}
		else {
			from = (int)(t / (opt_->T / opt_->nbTimeSteps)) + 1;
		}

		for (int j = 0; j < mod_->size_; j++) {
			mod_->shiftPath(path, pathMinus, pathPlus, j, from, opt_->nbTimeSteps, 0.01);
			payoffPlus = opt_->payoff(pathPlus);
			payoffMinus = opt_->payoff(pathMinus);

			LET(deltas,j) += (payoffPlus - payoffMinus) / (GET(current, j) * 2 * 0.01);
		}
	}

	for (int j = 0; j < mod_->size_; j++) {
		LET(deltas,j) /= nbSamples_;
		LET(deltas,j) *= exp(-mod_->r_ * (opt_->T - t));
	}

	pnl_mat_free(&path);
	pnl_mat_free(&pathMinus);
	pnl_mat_free(&pathPlus);

	return;
}