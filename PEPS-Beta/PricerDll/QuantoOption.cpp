#include "stdafx.h"
#include "QuantoOption.h"
#include <iostream>

QuantoOption::QuantoOption(double T, double strike)
{
	strike_ = strike; // taille size_
	this->custom = false;
	this->T = T;
	this->nbTimeSteps = 1;
	this->size = 1;
}

QuantoOption::~QuantoOption()
{
}

double QuantoOption::payoff(const PnlMat * path)
{
	double vFinale = MGET(path, path->m - 1, 0) * MGET(path, path->m - 1, 1);
	return max(0, vFinale - strike_);
	
}
