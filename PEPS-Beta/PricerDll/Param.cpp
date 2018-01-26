#pragma once


#include "stdafx.h"

#include "Param.h"
#include <math.h>

Param::Param(PnlMat * e_data):data(e_data)
{
	computeReturns();
	computeMean();
	computeCovMat();
	computeCorrMat();
	computeVol();
}

Param::~Param()
{
	pnl_mat_free(&returns);
	pnl_mat_free(&corrMat);
	pnl_mat_free(&covMat);
	pnl_vect_free(&mean);
	pnl_vect_free(&vol);
}

void Param::computeReturns()
{
	returns = pnl_mat_create_from_zero(data->m - 1, data->n);
	for (int j = 0; j < returns->n; j++) {
		for (int i = 0; i < returns->m; i++) {
			MLET(returns, i, j) = (MGET(data, i + 1, j) - MGET(data, i, j)) / MGET(data, i, j);
		}
	}
}

void Param::computeCovMat()
{
	covMat = pnl_mat_create_from_zero(data->n, data->n);
	for (int i = 0; i < data->n; i++) {
		double temp1 = GET(mean, i);
		for (int j = 0; j <= i; j++) {
			double temp2 = GET(mean, j);
			double res = 0;
			for (int k = 0; k < returns->m; k++) {
				res += (MGET(returns, k, i) - temp1)*(MGET(returns, k, j) - temp2);
			}
			MLET(covMat, i, j) = res / returns->m;
			MLET(covMat, j, i) = MGET(covMat, i, j);
		}
	}
}

void Param::computeCorrMat()
{
	corrMat = pnl_mat_create_from_zero(data->n, data->n);
	for (int i = 0; i < data->n; i++) {
		for (int j=0;j<=i;j++){
			MLET(corrMat, i, j) = MGET(covMat, i, j) / (GET(vol, i)*GET(vol, j));
			MLET(corrMat, j, i) = MGET(corrMat, i, j);
		}
	}
}

void Param::computeVol()
{
	vol = pnl_vect_create_from_zero(data->n);
	for (int i = 0; i < data->n; i++) {
		LET(vol, i) = sqrt(MGET(covMat, i, i));
	}
}

void Param::computeMean()
{
	mean = pnl_vect_create_from_zero(data->n);
	for (int j = 0; j < data->n; j++) {
		double temp = 0;
		for (int i = 0; i < data->m -1; i++) {
			temp += MGET(returns, i, j);
		}
		LET(mean, j) = temp/data->n;
	}
}
