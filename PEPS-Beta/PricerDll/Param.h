#pragma once

#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

class Param 
{
public :
	PnlMat * data;
	PnlMat * returns;
	PnlVect* mean;
	PnlVect* vol;
	PnlMat* corrMat;
	PnlMat* covMat;

	Param(PnlMat* e_data);
	~Param();

	void computeReturns();
	void computeCovMat();
	void computeCorrMat();
	void computeVol();	
	void computeMean();



};