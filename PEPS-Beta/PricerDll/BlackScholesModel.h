#pragma once

#include "pnl/pnl_random.h"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

/// \brief Mod�le de Black Scholes
class BlackScholesModel
{
	PnlMat *gammaMemSpace_;
	PnlMat *gMemSpace_;
	PnlVect tempMemSpace1_;
	PnlVect tempMemSpace2_;
public:
	int size_; /// nombre d'actifs du mod�le
	double r_; /// taux d'int�r�t
	double rho_; /// param�tre de corr�lation
	PnlVect *sigma_; /// vecteur de volatilit�s
	PnlVect *spot_; /// valeurs initiales du sous-jacent

	PnlVect *trend_; /// tendance du mod�le

	BlackScholesModel();
	BlackScholesModel(int size, double r, double rho, PnlVect *sigma,
		PnlVect *spot, PnlVect *trend);
	BlackScholesModel(const BlackScholesModel & bsm);
	virtual ~BlackScholesModel();


	/**
	* G�n�re une trajectoire du mod�le et la stocke dans path
	*
	* @param[out] path contient une trajectoire du mod�le.
	* C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturit�
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void asset(PnlMat *path, double T, int nbTimeSteps, PnlRng *rng);

	/**
	* Dans le cas d'appel d'asset successifs avec la configuration actuelle,
	* initialise les zones m�morielles d�di�es � asset avec les matrices ou
	* vecteurs de bonnes dimensions.
	*
	*  @param[in] nbTimeSteps nombre de dates de constatation
	*/
	void initAsset(int nbTimeSteps);

	/**
	* G�n�re une trajectoire du mod�le et la stocke dans path. Les appels doivent �tre fait apr�s un initAsset.
	*
	* @param[out] path contient une trajectoire du mod�le.
	* C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturit�
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void postInitAsset(PnlMat *path, double T, int nbTimeSteps, PnlRng *rng);

	/**
	* @brief Shift d'une trajectoire du sous-jacent
	*
	* @param[in]  path contient en input la trajectoire
	* du sous-jacent
	* @param[out] shift_path contient la trajectoire path
	* dont la composante d a �t� shift�e par (1+h)
	* � partir de la date t.
	* @param[in] t date � partir de laquelle on shift
	* @param[in] h pas de diff�rences finies
	* @param[in] d indice du sous-jacent � shifter
	* @param[in] timestep pas de constatation du sous-jacent
	*/
	void shiftAsset(PnlMat *shift_path, const PnlMat *path, int d,
		double h, double t, double timestep);

	/**
	* G�n�re une simulation du march�
	*
	* @param[out] market contient une trajectoire du mod�le.
	* C'est une matrice de taille (H+1) x d
	* @param[in] T maturit�
	* @param[in] H nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void simul_market(PnlMat *market, double T, int H, PnlRng *rng);
};
