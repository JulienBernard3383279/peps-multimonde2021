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

		mySum += opt_->payoff(path);
		mySquaredSum += pow(opt_->payoff(path), 2);
	}

	*prix = mySum / nbSamples_ * exp(- mod_->r_ * opt_->T);

	var = exp(-mod_->r_ * opt_->T * 2)
		* (mySquaredSum / nbSamples_ - pow(mySum / nbSamples_, 2));

	*ic = 2 * 1.96 * sqrt(var) / sqrt(nbSamples_);

	// Free memory
	pnl_mat_free(&path);
}

void MonteCarlo::deltas(double* deltas) {
	for (int i = 0; i < mod_->size_; i++) {
		deltas[i] = 0;
	}

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
			
			
			deltas[j] += (payoffPlus - payoffMinus) / (MGET(path, 0, j) * 2 * 0.01);
		}

	}

	for (int j = 0; j < mod_->size_; j++) {
		deltas[j] /= nbSamples_;
		deltas[j] *= exp(-mod_->r_ * opt_->T);
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
	for (int i = 0; i < nbSamples_; ++i) {
		if (!opt_->custom) { mod_->postInitAsset(path, 
			                                     past, t, current,
			                                     opt_->T, opt_->nbTimeSteps, rng_); }
		else { mod_->postInitAssetCustomDates(path,
			                                  past, t, current,
			                                  opt_->customDates, opt_->nbTimeSteps, rng_); }

		mySum += opt_->payoff(path);
		mySquaredSum += pow(opt_->payoff(path), 2);
	}

	*prix = mySum / nbSamples_ * exp(-mod_->r_ * (opt_->T - t) );

	var = exp(-mod_->r_ * (opt_->T - t )* 2)
		* (mySquaredSum / nbSamples_ - pow(mySum / nbSamples_, 2));

	*ic = 2 * 1.96 * sqrt(var) / sqrt(nbSamples_);

	// Free memory
	pnl_mat_free(&path);
}
