#pragma once

#include "stdafx.h"
#include <cmath>
#include "pnl/pnl_matrix.h"

#pragma region Volatility utils
/*
* Computes the volatility of A+B with B assuming A and B follow normal laws
*/
static double VolAplusB(double corrAB, double volA, double volB) {
	return sqrt(volA*volA + volB * volB + 2 * corrAB*volA*volB);
}

/*
* Computes the volatility of A-B with B assuming A and B follow normal laws
*/
static double VolAminusB(double corrAB, double volA, double volB) {
	return sqrt(volA*volA + volB * volB - 2 * corrAB*volA*volB);
}
#pragma endregion

#pragma region Correlation utils
/*
* Computes the volatility of A+B with C assuming A, B and C follow normal laws
*/
static double CorrAplusBwithC(double corrAB, double corrAC, double corrBC, double volA, double volB) {
	return (corrAC*volA + corrBC * volB) / VolAplusB(corrAB, volA, volB);
}

/*
* Computes the volatility of A-B with C assuming A, B and C follow normal laws
*/
static double CorrAminusBwithC(double corrAB, double corrAC, double corrBC, double volA, double volB) {
	return (corrAC*volA - corrBC * volB) / VolAminusB(corrAB, volA, volB);
}

/*
* Computes the volatility of A+B with C+D assuming A, B, C and D follow normal laws
*/
static double CorrAplusBwithCplusD(double corrAB, double corrAC, double corrAD, double corrBC, double corrBD, double corrCD,
	double volA, double volB, double volC, double volD) {
	return (
		volA * volC * corrAC +
		volA * volD * corrAD +
		volB * volC * corrBC +
		volB * volD * corrBD
		) / (
			VolAplusB(corrAB, volA, volB) +
			VolAplusB(corrCD, volC, volD)
			);
}

/*
* Computes the volatility of A-B with C+D assuming A, B, C and D follow normal laws
*/
static double CorrAminusBwithCplusD(double corrAB, double corrAC, double corrAD, double corrBC, double corrBD, double corrCD,
	double volA, double volB, double volC, double volD) {
	return (
		volA * volC * corrAC +
		volA * volD * corrAD -
		volB * volC * corrBC -
		volB * volD * corrBD
		) / (
			VolAminusB(corrAB, volA, volB) +
			VolAplusB(corrCD, volC, volD)
			);
}

/*
* Computes the volatility of A+B with C-D assuming A, B, C and D follow normal laws
*/
static double CorrAplusBwithCminusD(double corrAB, double corrAC, double corrAD, double corrBC, double corrBD, double corrCD,
	double volA, double volB, double volC, double volD) {
	return (
		volA * volC * corrAC -
		volA * volD * corrAD +
		volB * volC * corrBC -
		volB * volD * corrBD
		) / (
			VolAplusB(corrAB, volA, volB) +
			VolAminusB(corrCD, volC, volD)
			);
}

/*
* Computes the volatility of A-B with C-D assuming A, B, C and D follow normal laws
*/
static double CorrAminusBwithCminusD(double corrAB, double corrAC, double corrAD, double corrBC, double corrBD, double corrCD,
	double volA, double volB, double volC, double volD) {
	return (
		volA * volC * corrAC -
		volA * volD * corrAD -
		volB * volC * corrBC +
		volB * volD * corrBD
		) / (
			VolAminusB(corrAB, volA, volB) +
			VolAminusB(corrCD, volC, volD)
			);
}

/*
 * Computes the correlation of A-B with B assuming A and B follow normal laws
 * For compatibility
 */
static double CorrAminusBwithB(double corrAB, double volA, double volB)
{
	return (corrAB * volA - volB) / sqrt(volA * volA + volB * volB - 2 * corrAB * volA * volB);
}

/*
* Computes the correlation of A+B with B assuming A and B follow normal laws
* For compatibility
*/
static double CorrAplusBwithB(double corrAB, double volA, double volB)
{
	return (corrAB * volA + volB) / sqrt(volA * volA + volB * volB + 2 * corrAB * volA * volB);
}
#pragma endregion

#pragma region Quanto option utils
/*
* Takes a 2x2 correlation matrix for variables A and B and
* transforms it into the correlation matrix of variables A+B and B
* Uses an array
*/
static void FromCorrABToCorrAPlusBB_old(double* matrix, double volA, double volB)
{
	matrix[1] = CorrAminusBwithB(matrix[1], volA, volB);
	matrix[2] = CorrAminusBwithB(matrix[2], volA, volB);
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
	MLET(matrix, 0, 1) = CorrAminusBwithB(MGET(matrix, 0, 1), volA, volB);
	MLET(matrix, 1, 0) = CorrAminusBwithB(MGET(matrix, 1, 0), volA, volB);
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
#pragma endregion