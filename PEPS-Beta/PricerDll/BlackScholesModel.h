#pragma once

#include "pnl/pnl_random.h"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

/// \brief Modèle de Black Scholes
class BlackScholesModel
{
	PnlMat *gammaMemSpace_;
	PnlMat *gMemSpace_;
	PnlVect tempMemSpace1_;
	PnlVect tempMemSpace2_;
public:
	int size_; /// nombre d'actifs du modèle
	double r_; /// taux d'intérêt
	PnlMat *correlations_; /// paramètre de corrélation
	PnlVect *sigma_; /// vecteur de volatilités
	PnlVect *spot_; /// valeurs initiales du sous-jacent
	PnlVect *trend_; /// tendance du modèle

	BlackScholesModel();
	BlackScholesModel(int size, double r, PnlMat *correlations, PnlVect *sigma,
		PnlVect *spot, PnlVect *trend);
	BlackScholesModel(const BlackScholesModel & bsm);
	virtual ~BlackScholesModel();


	/**
	* Génère une trajectoire du modèle et la stocke dans path
	*
	* @param[out] path contient une trajectoire du modèle.
	* C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturité
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng générateur de nombres aléatoires
	*/
	void asset(PnlMat *path, double T, int nbTimeSteps, PnlRng *rng);

	/**
	* Dans le cas d'appel d'asset successifs avec la configuration actuelle,
	* initialise les zones mémorielles dédiées à asset avec les matrices ou
	* vecteurs de bonnes dimensions.
	*
	*  @param[in] nbTimeSteps nombre de dates de constatation
	*/
	void initAsset(int nbTimeSteps);

	/**
	* Génère une trajectoire du modèle et la stocke dans path. Les appels doivent être fait après un initAsset.
	*
	* @param[out] path contient une trajectoire du modèle. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturité
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng générateur de nombres aléatoires
	*/
	void postInitAsset(PnlMat *path, 
		double T, int nbTimeSteps, PnlRng *rng);

	/**
	* Génère une trajectoire du modèle et la stocke dans path. Les appels doivent être fait après un initAsset.
	*
	* @param[out] path contient une trajectoire du modèle. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] past contient la trajectoire déjà fixée du modèle.
	* @param[in] t contient la date actuelle (tarée avec dates[0])
	* @param[in] current contient les prix actuels
	* @param[in] T  maturité
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng générateur de nombres aléatoires
	*/
	void postInitAsset(PnlMat *path, 
		PnlMat *past, double t, PnlVect *current,
		double T, int nbTimeSteps, PnlRng *rng);

	/**
	* Génère une trajectoire du modèle et la stocke dans path. Les appels doivent être fait après un initAsset.
	* Cette fonction utilise, au lieu d'une maturité fixe, les dates de constations auxquelles nous devons générer les prix des indices.
	* L'array doit être de taille nbTimeSteps+1.
	*
	* @param[out] path contient une trajectoire du modèle. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] T  maturité
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng générateur de nombres aléatoires
	*/
	void postInitAssetCustomDates(PnlMat *path, 
		PnlVect* dates, int nbTimeSteps, PnlRng *rng);
	
	/**
	* Génère une trajectoire du modèle et la stocke dans path. Les appels doivent être fait après un initAsset.
	* Cette fonction utilise, au lieu d'une maturité fixe, les dates de constations auxquelles nous devons générer les prix des indices.
	* L'array doit être de taille nbTimeSteps+1.
	*
	* @param[out] path contient une trajectoire du modèle. C'est une matrice de taille (nbTimeSteps+1) x d
	* @param[in] past contient la trajectoire déjà fixée du modèle.
	* @param[in] t contient la date actuelle (tarée avec dates[0])
	* @param[in] current contient les prix actuels
	* @param[in] dates dates de constation customs
	* @param[in] nbTimeSteps nombre de dates de constatation
	* @param[in] rng générateur de nombres aléatoires
	*/
	void postInitAssetCustomDates(PnlMat *path, 
		PnlMat *past, double t, PnlVect *current,
		PnlVect* dates, int nbTimeSteps, PnlRng *rng);

	/*
	* Cette fonction shift la (i+1)ème colonne de la trajectoire de (1-h) ; (1+h) à partir de l'indice from.
	* 
	* @param[in] path  contient le path initial.
	* @param[out] pathMinus contiendra le path shifté de -h après exécution. La matrice doit être déclarée au préalable.
	* @param[out] pathPlus contiendra le path shifté de +h après exécution. La matrice doit être déclarée au préalable.
	* @param[in] j contient l'indice de l'actif à shifter.
	* @param[in] from contient le premier indice à shifter.
	* @param[in] nbTimeSteps est le nombre de dates de constation (hauteur de la matrice - 1)
	* @param[in] h est l'intensité de shifting
	*/
	void shiftPath(PnlMat* path, PnlMat *pathMinus, PnlMat *pathPlus, int j, int from, int nbTimeSteps, double h);
};
