#pragma once

#ifdef UNMANAGEDCPPDLL_EXPORTS
#define UNMANAGEDCPPDLL_API __declspec(dllexport)
#else
#define UNMANAGEDCPPDLL_API __declspec(dllimport)
#endif

extern "C" UNMANAGEDCPPDLL_API double SendDouble(double d);
