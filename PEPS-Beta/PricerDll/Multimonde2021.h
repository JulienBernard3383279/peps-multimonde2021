#pragma once
#include "Option.h"

/*
* CE MULTIMONDE N'EST PAS LE BON. LE CORRECT CALCULATOIREMENT EST MULTIMONDE2021QUANTO
*/

class Multimonde2021 :
	public Option
{
	PnlVect* payoffVectMemSpaceInit_;
	PnlVect* payoffVectMemSpaceCurrent_;
public:
	Multimonde2021(PnlVect* customDates);
	~Multimonde2021();

	double payoff(const PnlMat *path);

	double verbosePayoff(const PnlMat *path);
};


