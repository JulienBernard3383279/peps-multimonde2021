#pragma once

#include "stdafx.h"
#include <cmath>
#include "pnl/pnl_matrix.h"

/*
* Compute the correlation of A+B with B assuming A and B follow normal laws
*/
static double CorrAplusBwithB(double corrAB, double volA, double volB)
{
	return (corrAB * volA - volB) / sqrt(volA * volA + volB * volB - 2 * corrAB * volA * volB);
}

/*
* Takes a 2x2 correlation matrix for variables A and B and
* transforms it into the correlation matrix of variables A+B and B
* Uses an array
*/
static void FromCorrABToCorrAPlusBB_old(double* matrix, double volA, double volB)
{
	matrix[1] = CorrAplusBwithB(matrix[1], volA, volB);
	matrix[2] = CorrAplusBwithB(matrix[2], volA, volB);
}

/*
* Generates a 2x2 correlation matrix for variables A+B and B
* from the correlation matrix of variables A+B and B
* Uses an array
*/
static double* GenCorrAPlusBBFromCorrAB_old(double* matrix, double volA, double volB)
{
	double* toBeReturned = new double[4];
	toBeReturned[0] = matrix[0];
	toBeReturned[3] = matrix[3];
	FromCorrABToCorrAPlusBB_old(toBeReturned, volA, volB);
	return toBeReturned;
}
static double* GenCorrAPlusBBFromCorrAB_old(double* matrix, double* vol)
{
	return GenCorrAPlusBBFromCorrAB_old(matrix, vol[0], vol[1]);
}

/*
* Takes a 2x2 correlation matrix for variables A and B and
* transforms it into the correlation matrix of variables A+B and B
* Uses an array
*/
static void FromCorrABToCorrAPlusBB(PnlMat* matrix, double volA, double volB)
{
	MLET(matrix, 0, 1) = CorrAplusBwithB(MGET(matrix, 0, 1), volA, volB);
	MLET(matrix, 1, 0) = CorrAplusBwithB(MGET(matrix, 1, 0), volA, volB);
}

/*
* Generates a 2x2 correlation matrix for variables A+B and B
* from the correlation matrix of variables A+B and B
* Uses an array
*/
static PnlMat* GenCorrAPlusBBFromCorrAB(double* matrix, double volA, double volB)
{
	PnlMat* toBeReturned = pnl_mat_create(2, 2);
	MLET(toBeReturned, 0, 0) = matrix[0];
	MLET(toBeReturned, 0, 1) = matrix[1];
	MLET(toBeReturned, 1, 0) = matrix[2];
	MLET(toBeReturned, 1, 1) = matrix[3];

	FromCorrABToCorrAPlusBB(toBeReturned, volA, volB);
	return toBeReturned;
}
static PnlMat* GenCorrAPlusBBFromCorrAB(double* matrix, double* vol)
{
	return GenCorrAPlusBBFromCorrAB(matrix, vol[0], vol[1]);
}