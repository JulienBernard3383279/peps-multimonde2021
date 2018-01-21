#pragma once

#include "stdafx.h"

#include <iostream>
#include <cmath>

#include "MonteCarlo.h"

using namespace std;

MonteCarlo::MonteCarlo() {}

MonteCarlo::MonteCarlo(BlackScholesModel *mod, Option *opt, PnlRng *rng, int nbSamples) {
	mod_ = mod;
	opt_ = opt;
	rng_ = rng;
	nbSamples_ = nbSamples;
}

MonteCarlo::~MonteCarlo() {}

void MonteCarlo::price(double &prix, double &ic) {

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

	std::cout << exp(-mod_->r_ * opt_->T) << std::endl;
	prix = mySum / nbSamples_ * exp(- mod_->r_ * opt_->T);

	var = exp(-mod_->r_ * opt_->T * 2)
		* (mySquaredSum / nbSamples_ - pow(mySum / nbSamples_, 2));

	ic = 2 * 1.96 * sqrt(var) / sqrt(nbSamples_);

	// Free memory
	pnl_mat_free(&path);
}