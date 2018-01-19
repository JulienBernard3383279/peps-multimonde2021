// UnmanagedCppDll.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include "UnmanagedCppDll.h"
#include "pnl/pnl_vector.h"

double SendDouble(double d) {
	PnlVect* vect = pnl_vect_create_from_double(2, d);
	return pnl_vect_sum(vect);
}