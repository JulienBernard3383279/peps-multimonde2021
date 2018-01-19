#pragma once

#include "stdafx.h"

#include <iostream>
#include <cmath>

#include "BlackScholesModel.h"


using namespace std;

/**
* \brief Constructeur par d�faut de la classe BlackScholesModel
*/
BlackScholesModel::BlackScholesModel() {
	size_ = 0;
	r_ = 0;
	rho_ = 0;
	sigma_ = pnl_vect_new();
	spot_ = pnl_vect_new();
	trend_ = pnl_vect_new();

	gammaMemSpace_ = pnl_mat_new();
	gMemSpace_ = pnl_mat_new();
}

/**
* \brief Constructeur avec arguments de la classe BlackScholesModel
*/
BlackScholesModel::BlackScholesModel(int size, double r, double rho,
	PnlVect *sigma, PnlVect *spot, PnlVect *trend) {
	size_ = size;
	r_ = r;
	rho_ = rho;
	sigma_ = pnl_vect_copy(sigma);
	spot_ = pnl_vect_copy(spot);
	trend_ = pnl_vect_copy(trend);

	gammaMemSpace_ = pnl_mat_create_from_scalar(size, size, rho);
	gMemSpace_ = pnl_mat_new();
}

/**
* \brief Constructeur par recopie de la classe BlackScholesModel
*/
BlackScholesModel::BlackScholesModel(const BlackScholesModel &bsm) {
	size_ = bsm.size_;
	r_ = bsm.r_;
	rho_ = bsm.rho_;
	sigma_ = pnl_vect_copy(bsm.sigma_);
	spot_ = pnl_vect_copy(bsm.spot_);
	trend_ = pnl_vect_copy(bsm.trend_);
	gammaMemSpace_ = pnl_mat_copy(bsm.gammaMemSpace_);
	gMemSpace_ = pnl_mat_copy(bsm.gMemSpace_);
}

/**
* \brief Destructeur de la classe BlackScholesModel
*/
BlackScholesModel::~BlackScholesModel() {
	pnl_vect_free(&sigma_);
	pnl_vect_free(&spot_);
	pnl_vect_free(&trend_);
	pnl_mat_free(&gMemSpace_);
	pnl_mat_free(&gammaMemSpace_);
}

void BlackScholesModel::initAsset(int nbTimeSteps) {
	// Cr�ation de la matrice Gamma
	gammaMemSpace_ = pnl_mat_create_from_scalar(size_, size_, rho_);
	for (int d = 0; d < size_; ++d) {
		MLET(gammaMemSpace_, d, d) = 1;
	}

	// R�cup�ration de la matrice L (factoris�e de Cholesky de Gamma)
	int info = pnl_mat_chol(gammaMemSpace_);
	gMemSpace_ = pnl_mat_create(nbTimeSteps + 1, size_);
}

void BlackScholesModel::postInitAsset(PnlMat *path, double T, int nbTimeSteps, PnlRng *rng) {
	// Initialisation de path
	pnl_mat_set_row(path, spot_, 0);

	pnl_mat_rng_normal(gMemSpace_, nbTimeSteps + 1, size_, rng);

	double step = T / nbTimeSteps;

	for (int i = 1; i <= nbTimeSteps; ++i) {
		for (int d = 0; d < size_; ++d) {
			tempMemSpace1_ = pnl_vect_wrap_mat_row(gammaMemSpace_, d);
			tempMemSpace2_ = pnl_vect_wrap_mat_row(gMemSpace_, i);

			MLET(path, i, d) = MGET(path, i - 1, d)
				* exp((r_ - pow(GET(sigma_, d), 2) / 2.) * step
					+ GET(sigma_, d) * sqrt(step) * pnl_vect_scalar_prod(&tempMemSpace1_, &tempMemSpace2_));
		}
	}
}

/**
* G�n�re une trajectoire du mod�le et la stocke dans path
*/
void BlackScholesModel::asset(PnlMat *path, double T, int nbTimeSteps, PnlRng *rng) {
	initAsset(nbTimeSteps);
	postInitAsset(path, T, nbTimeSteps, rng);
}

/**
* @brief Shift d'une trajectoire du sous-jacent
*/
void BlackScholesModel::shiftAsset(PnlMat *shift_path, const PnlMat *path,
	int d, double h, double t, double timestep) {
	// Clonage de la matrice path
	pnl_mat_clone(shift_path, path);

	// Shift
	int start = (int)(t / timestep); // index � partir duquel on shift
	for (int i = start; i < path->m; ++i) {
		MLET(shift_path, i, d) *= (1 + h);
	}
}

/**
* @brief Simule des donn�es march�
*/
void BlackScholesModel::simul_market(PnlMat *market, double T, int H, PnlRng *rng) {
	// Cr�ation de la matrice Gamma
	PnlMat *gamma = pnl_mat_create_from_scalar(size_, size_, rho_);
	for (int d = 0; d < size_; ++d) {
		MLET(gamma, d, d) = 1;
	}

	// R�cup�ration de la matrice L (factoris�e de Cholesky de Gamma)
	int info = pnl_mat_chol(gamma);

	// Initialisation de path
	pnl_mat_set_row(market, spot_, 0);

	// Cr�ation de la matrice G gaussienne
	PnlMat *g = pnl_mat_create(H, size_);
	pnl_mat_rng_normal(g, H, size_, rng);

	double step = T / (H - 1);

	for (int i = 1; i < H; ++i) {
		for (int d = 0; d < size_; ++d) {
			PnlVect temp = pnl_vect_wrap_mat_row(gamma, d);
			PnlVect temp1 = pnl_vect_wrap_mat_row(g, i);

			MLET(market, i, d) = MGET(market, i - 1, d)
				* exp((GET(trend_, d) - pow(GET(sigma_, d), 2) / 2.) * step
					+ GET(sigma_, d) * sqrt(step) * pnl_vect_scalar_prod(&temp, &temp1));
		}

	}

	// Free memory
	pnl_mat_free(&g);
	pnl_mat_free(&gamma);
}
