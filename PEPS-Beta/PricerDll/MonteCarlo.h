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
	void price(double &prix, double &ic);
};
