#pragma once

#include "Option.h"
#include "BlackScholesModel.h"

#include "pnl/pnl_random.h"
#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"

class MonteCarlo
{
public:
	BlackScholesModel * mod_; /*! pointeur vers le modèle */
	Option *opt_; /*! pointeur sur l'option */
	PnlRng *rng_; /*! pointeur sur le générateur */
	int nbSamples_; /*! nombre de tirages Monte Carlo */

	MonteCarlo();

	MonteCarlo(BlackScholesModel *mod, Option *opt, PnlRng *rng, int nbSamples);

	virtual ~MonteCarlo();

	/**
	* Calcule le prix de l'option à la date 0
	*
	* @param[out] prix valeur de l'estimateur Monte Carlo
	* @param[out] ic largeur de l'intervalle de confiance
	*/
	void price(double* prix, double* ic);

	/**
	* Calcule les deltas de l'option à la date 0. 
	*
	* @param[out] Vecteur de taille opt->size_ contenant les deltas. Doit être préalablement alloué et ne contenir que des 0.
	*/
	void deltas(PnlVect* deltas);

	/**
	* Calcule le prix de l'option en toute date t.
	*
	* @param[in] past le path jusqu'à présent
	* @param[in] t la date actuelle
	* @param[out] prix valeur de l'estimateur Monte Carlo
	* @param[out] ic largeur de l'intervalle de confiance
	*/
	void price(PnlMat* past, double t, PnlVect* current, double* prix, double* ic);

	/**
	* Calcule les deltas de l'option en toute date t.
	*
	* @param[in] past le path jusqu'à présent
	* @param[in] t la date actuelle
	* @param[out] Vecteur de taille opt->size_ contenant les deltas. Doit être préalablement alloué et ne contenir que des 0.
	*/
	void deltas(PnlMat *past, double t, PnlVect* current, PnlVect* deltas);

	/**
	* Calcule des deltas de S et X au lieu de ceux de S/X et 1/X. A utiliser avec le Multimonde2021Quanto.
	*
	* @param[in] past le path jusqu'à présent
	* @param[in] t la date actuelle
	* @param[out] Vecteur de taille opt->size_ contenant les deltas. Doit être préalablement alloué et ne contenir que des 0.
	*/
	void deltasMultimonde2021Quanto(PnlMat *past, double t, PnlVect* current, PnlVect* deltas);
};
