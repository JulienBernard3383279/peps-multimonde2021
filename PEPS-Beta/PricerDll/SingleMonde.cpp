#include "stdafx.h"
#include "SingleMonde.h"
#include <iostream>
#include <cstring>

SingleMonde::SingleMonde() {
	this->custom = true;
	this->T = 371.0/365.25;
	this->nbTimeSteps = 1;
	this->size = 1;

}

SingleMonde::~SingleMonde() {
}

double SingleMonde::payoff(const PnlMat* path) { 
	double perf = MGET(path, 1, 0) / MGET(path, 0, 0);
	return 100 * (perf < 0.85 ? 0.85 : perf > 1.15 ? 1.15 : perf);
	//Code d'alexandra, commenté pour la postérité
	//path taille 1,1, représentant la liste suivante :
	/*
	* 01/10/15
	*//*double globalPerf = 1.0;

	double max;
	max = 0.0;

	if (max >= 0.85 && max <= 1.15) {
		globalPerf = max - 1.0;
	}
	else {
		globalPerf = (max < 1 ? -0.15 : 0.15);
	}


	return 100 * globalPerf;*/
}

double SingleMonde::verbosePayoff() { //path taille 1,1, représentant la liste suivante :
														   /*
														   * 01/10/15
														   */

	double globalPerf = 1.0;
	double max;

	max = 0.0;

	if (max >= 0.85 && max <= 1.15) {
		globalPerf = max - 1.0;
	}
	else {
		globalPerf = (max < 1 ? -0.15 : 0.15);
	}
	return 100 * globalPerf;
}