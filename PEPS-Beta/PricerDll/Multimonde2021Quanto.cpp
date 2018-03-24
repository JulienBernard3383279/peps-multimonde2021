#include "stdafx.h"
#include "Multimonde2021Quanto.h"
#include <iostream>

Multimonde2021Quanto::Multimonde2021Quanto(double interestRates[6])
{
	payoffVectMemSpaceInit_ = pnl_vect_create_from_zero(11);
	payoffVectMemSpaceCurrent_ = pnl_vect_create_from_zero(11);
	this->custom = true;
	double temp = 371.0 / 365.25;
	this->customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	this->size = 11; //Actif euro [1] //Actifs étrangers [5] // Taux de change [5] : 1 € en monnaie étrangère
	this->nbTimeSteps = 6;
	this->T = GET(customDates, 6);
	this->interestRates = interestRates;
}


Multimonde2021Quanto::~Multimonde2021Quanto()
{
	pnl_vect_free(&payoffVectMemSpaceInit_);
	pnl_vect_free(&payoffVectMemSpaceCurrent_);
	pnl_vect_free(&(this->customDates));
}

double Multimonde2021Quanto::payoff(const PnlMat *path) {
	/*
		* 01/10/15
		* 07/10/16
		* 13/10/17
		* 19/10/18
		* 25/10/19
		* 30/10/20
		* 05/11/21
		*/

	//Récupération des spots
	pnl_mat_get_row(payoffVectMemSpaceInit_, path, 0);

	//Mises dans leurs monnaies (Les actifs sont SX)
	PnlVect* temp = pnl_vect_create_from_list(11,
		1.0,
		MGET(path, 0, 6)*exp(interestRates[1]*(this->T)), // Multiplication par le taux de change
		MGET(path, 0, 7)*exp(interestRates[2] * (this->T)), // Taux de change = zéro-coupon actualisé à T
		MGET(path, 0, 8)*exp(interestRates[3] * (this->T)),
		MGET(path, 0, 9)*exp(interestRates[4] * (this->T)),
		MGET(path, 0, 10)*exp(interestRates[5] * (this->T)),
		1.0,
		1.0,
		1.0,
		1.0,
		1.0);
	pnl_vect_mult_vect_term(payoffVectMemSpaceInit_, temp);
	pnl_vect_free(&temp);

	//Init
	double globalPerf = 1.0;
	bool stillHere[6];
	for (int i = 0; i < 6; i++) stillHere[i] = true;
	double max;
	int maxIndex;

	//Itération sur les actifs
	for (int i = 1; i <= 6; ++i) { // i = le temps, itère dans les dates de constatation, [1,6] normal
		max = 0;
		maxIndex = 0;
		pnl_mat_get_row(payoffVectMemSpaceCurrent_, path, i);
		
		//Remise dans leurs monnaies+ étrangères (opti : ne simuler que S pour le payoff, et ne faire ça que pour les actifs encore en lice)
		PnlVect* temp = pnl_vect_create_from_list(11,
			1.0,
			MGET(path, i, 6)*exp(interestRates[1] * (this->T - GET(customDates,i))),
			MGET(path, i, 7)*exp(interestRates[2] * (this->T) - GET(customDates, i)),
			MGET(path, i, 8)*exp(interestRates[3] * (this->T) - GET(customDates, i)),
			MGET(path, i, 9)*exp(interestRates[4] * (this->T) - GET(customDates, i)),
			MGET(path, i, 10)*exp(interestRates[5] * (this->T) - GET(customDates, i)),
			1.0,
			1.0,
			1.0,
			1.0,
			1.0);
		pnl_vect_mult_vect_term(payoffVectMemSpaceCurrent_, temp);
		pnl_vect_free(&temp);

		//Division par les valeurs initiales
		pnl_vect_div_vect_term(payoffVectMemSpaceCurrent_, payoffVectMemSpaceInit_);

		//Maximum de ceux encore en lice
		for (int j = 0; j < 6; j++) {
			if (stillHere[j] && GET(payoffVectMemSpaceCurrent_, j) > max) {
				maxIndex = j;
				max = GET(payoffVectMemSpaceCurrent_, j);
			}
		}
		if (max >= 0.85 && max <= 1.15) {
			globalPerf += max - 1.0;
		}
		else {
			globalPerf += (max < 1 ? -0.15 : 0.15);
		}
		stillHere[maxIndex] = false;
	}
	return 100 * globalPerf;
}