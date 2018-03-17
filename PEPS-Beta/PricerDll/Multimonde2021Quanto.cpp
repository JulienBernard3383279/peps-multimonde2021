#include "stdafx.h"
#include "Multimonde2021Quanto.h"
#include <iostream>

Multimonde2021Quanto::Multimonde2021Quanto()
{
	payoffVectMemSpaceInit_ = pnl_vect_create_from_zero(6);
	payoffVectMemSpaceCurrent_ = pnl_vect_create_from_zero(6);
	this->custom = true;
	double temp = 371.0 / 365.25;
	this->customDates = pnl_vect_create_from_list(7, 0.0, temp, temp * 2, temp * 3, temp * 4, temp * 5, temp * 6);
	this->size = 11; //Actif euro [1] //Actifs �trangers [5] // Taux de change [5] : 1 � en monnaie �trang�re
	this->nbTimeSteps = 6;
	this->T = GET(customDates, 6);
}


Multimonde2021Quanto::~Multimonde2021Quanto()
{
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

	//R�cup�ration des spots
	pnl_mat_get_row(payoffVectMemSpaceInit_, path, 0);

	//Mises dans leurs monnaies
	pnl_vect_mult_vect_term(payoffVectMemSpaceInit_, pnl_vect_create_from_list(11,
		1.0,
		MGET(path, 0, 6),
		MGET(path, 0, 7),
		MGET(path, 0, 8),
		MGET(path, 0, 9),
		MGET(path, 0, 10),
		1.0,
		1.0,
		1.0,
		1.0,
		1.0));

	//Init
	double globalPerf = 1.0;
	bool stillHere[6];
	for (int i = 0; i < 6; i++) stillHere[i] = true;
	double max;
	int maxIndex;

	//It�ration sur les actifs
	for (int i = 1; i <= 6; ++i) { // i = le temps, it�re dans les dates de constation, [1,6] normal
		max = 0;
		maxIndex = 0;
		pnl_mat_get_row(payoffVectMemSpaceCurrent_, path, i);
		
		//Remise dans leurs monnaies �trang�res (opti : ne simuler que S pour le payoff, et ne faire �a que pour les actifs encore en lice)
		pnl_vect_mult_vect_term(payoffVectMemSpaceCurrent_, pnl_vect_create_from_list(11,
			1.0,
			MGET(path, i, 6),
			MGET(path, i, 7),
			MGET(path, i, 8),
			MGET(path, i, 9),
			MGET(path, i, 10),
			1.0,
			1.0,
			1.0,
			1.0,
			1.0));

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