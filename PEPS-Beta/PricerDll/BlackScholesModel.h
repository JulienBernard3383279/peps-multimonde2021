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
	PnlMat *correlations_; /// param�tre de corr�lation
	PnlVect *sigma_; /// vecteur de volatilit�s
	PnlVect *spot_; /// valeurs initiales du sous-jacent
	PnlVect *trend_; /// tendance du mod�le

	BlackScholesModel();
	BlackScholesModel(int size, double r, PnlMat *correlations, PnlVect *sigma,
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
	* @param[out] path contient une trajectoire du mod�le. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturit�
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void postInitAsset(PnlMat *path, 
		double T, int nbTimeSteps, PnlRng *rng);

	/**
	* G�n�re une trajectoire du mod�le et la stocke dans path. Les appels doivent �tre fait apr�s un initAsset.
	*
	* @param[out] path contient une trajectoire du mod�le. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] past contient la trajectoire d�j� fix�e du mod�le.
	* @param[in] t contient la date actuelle (tar�e avec dates[0])
	* @param[in] current contient les prix actuels
	* @param[in] T  maturit�
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void postInitAsset(PnlMat *path, 
		PnlMat *past, double t, PnlVect *current,
		double T, int nbTimeSteps, PnlRng *rng);

	/**
	* G�n�re une trajectoire du mod�le et la stocke dans path. Les appels doivent �tre fait apr�s un initAsset.
	* Cette fonction utilise, au lieu d'une maturit� fixe, les dates de constations auxquelles nous devons g�n�rer les prix des indices.
	* L'array doit �tre de taille nbTimeSteps+1.
	*
	* @param[out] path contient une trajectoire du mod�le. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturit�
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void postInitAssetCustomDates(PnlMat *path, 
		PnlVect* dates, int nbTimeSteps, PnlRng *rng);
	
	/**
	* G�n�re une trajectoire du mod�le et la stocke dans path. Les appels doivent �tre fait apr�s un initAsset.
	* Cette fonction utilise, au lieu d'une maturit� fixe, les dates de constations auxquelles nous devons g�n�rer les prix des indices.
	* L'array doit �tre de taille nbTimeSteps+1.
	*
	* @param[out] path contient une trajectoire du mod�le. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] past contient la trajectoire d�j� fix�e du mod�le.
	* @param[in] t contient la date actuelle (tar�e avec dates[0])
	* @param[in] current contient les prix actuels
	* @param[in] dates dates de constation customs
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng g�n�rateur de nombres al�atoires
	*/
	void postInitAssetCustomDates(PnlMat *path, 
		PnlMat *past, double t, PnlVect *current,
		PnlVect* dates, int nbTimeSteps, PnlRng *rng);

	/*
	* Cette fonction shift la (i+1)�me colonne de la trajectoire de (1-h) ; (1+h) � partir de l'indice from.
	* 
	* @param[in] path  contient le path initial.
	* @param[out] pathMinus contiendra le path shift� de -h apr�s ex�cution. La matrice doit �tre d�clar�e au pr�alable.
	* @param[out] pathPlus contiendra le path shift� de +h apr�s ex�cution. La matrice doit �tre d�clar�e au pr�alable.
	* @param[in] j contient l'indice de l'actif � shifter.
	* @param[in] from contient le premier indice � shifter.
	* @param[in] nbTimeSteps est le nombre de dates de constation (hauteur de la matrice - 1)
	* @param[in] h est l'intensit� de shifting
	*/
	void shiftPath(PnlMat* path, PnlMat *pathMinus, PnlMat *pathPlus, int j, int from, int nbTimeSteps, double h);
};
