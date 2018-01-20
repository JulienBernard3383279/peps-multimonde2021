#include "stdafx.h"
#include "Multimonde2021.h"
#include <iostream>
#include <cstring>

Multimonde2021::Multimonde2021(double* customDates) {
	payoffVectMemSpaceInit_ = pnl_vect_create(6);
	payoffVectMemSpaceCurrent_ = pnl_vect_create(6);
	this->customDates = customDates;
	this->size = 6;
	this->nbTimeSteps = 6;
	this->T = customDates[6];
}


Multimonde2021::~Multimonde2021() {

}

//TODO les valeurs doivent-elles �tre corrig�s de l'actualisation ? A voir dans les specs

double Multimonde2021::payoff(const PnlMat* path) { //path est obligatoirement une matrice de 6 de largeur et 7 de hauteur, repr�sentant la liste suivante :
	/*
	* 01/10/15
	* 07/10/16
	* 13/10/17
	* 19/10/18
	* 25/10/19
	* 30/10/20
	* 05/11/21
	*/
	pnl_mat_get_row(payoffVectMemSpaceInit_, path, 0);

	double globalPerf = 1.0;
	bool stillHere[6];
	for (int i = 1; i < 6; i++) stillHere[i] = true;

	double max;
	int maxIndex;

	for (int i = 1; i <= 6; ++i) {
		max = 0;
		maxIndex = 0;
		pnl_mat_get_row(payoffVectMemSpaceCurrent_, path, i);
		pnl_vect_div_vect_term(payoffVectMemSpaceCurrent_, payoffVectMemSpaceInit_);
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

	std::cout << 100 * globalPerf << std::endl;
	return 100 * globalPerf;
}