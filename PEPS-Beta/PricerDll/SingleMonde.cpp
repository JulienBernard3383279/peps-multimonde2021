#include "stdafx.h"
#include "SingleMonde.h"
#include <iostream>
#include <cstring>

SingleMonde::SingleMonde(double T) {
	this->custom = true;
	this->T = T;
	this->nbTimeSteps = 1;
	this->size = 1;

}



SingleMonde::~SingleMonde() {
}

double SingleMonde::payoff(const PnlMat* path) { //path taille 1,1, représentant la liste suivante :
													/*
													* 01/10/15

													*/


	double globalPerf = 1.0;
	double max;
	max = this->payoff;

	if (max >= 0.85 && max <= 1.15) {
		globalPerf = max - 1.0;
	}
	else {
		globalPerf = (max < 1 ? -0.15 : 0.15);
	}

	this->payoff = 100 * globalPerf;
	return 100 * globalPerf;
}

double SingleMonde::verbosePayoff(const PnlMat* path) { //path taille 1,1, représentant la liste suivante :
														   /*
														   * 01/10/15
														   */



	double globalPerf = 1.0;
	double max;

	max = this->payoff;

	if (max >= 0.85 && max <= 1.15) {
		globalPerf += max - 1.0;
	}
	else {
		globalPerf += (max < 1 ? -0.15 : 0.15);
	}

	return 100 * globalPerf;
}