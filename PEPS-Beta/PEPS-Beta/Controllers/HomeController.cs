using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace PEPS_Beta.Controllers
{
    public class HomeController : Controller
    {
        [DllImport(@"C:\Users\Julien\Desktop\PEPS-2017-2018\PEPS-Beta\x64\Debug\PricerDll.dll")]
        extern unsafe static void PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends,
            double* price,
            double* ic
        );

        // GET: Home
        public unsafe ActionResult Index()
        {
            int optionSize = 40;
            double[] payoffCoefficients = new double[optionSize];
            double[] spots = new double[optionSize];
            double[] volatilities = new double[optionSize];
            double[] trends = new double[optionSize];
            for (int i = 0; i < optionSize; i++)
            {
                payoffCoefficients[i] = 0.025;
                spots[i] = 100;
                volatilities[i] = 0.2;
                trends[i] = 0;
            }

            double[] correlations = new double[optionSize * optionSize];
            for (int i = 0; i < optionSize; i++)
            {
                for (int j = 0; j < optionSize; j++)
                {
                    correlations[i * optionSize + j] = (i == j) ? 1 : 0;
                }
            }

            double price;
            double ic;
            PriceMultimonde2021(
                100000,
                spots,
                volatilities,
                0.0,
                correlations,
                trends,
                &price,
                &ic);

            ViewData["d"] = price;
            return View();
        }
    }
}