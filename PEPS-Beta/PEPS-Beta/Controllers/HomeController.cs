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
        [DllImport(@"..\..\x64\Debug\PricerDll.dll")] 
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
            // ParseData
            Models.DataStorage ds = new Models.DataStorage();
            ds.FillData();
            ds.DataToArray();
            Models.DataCurrencies dc = new Models.DataCurrencies();
            dc.Fill();
            //

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
            ViewData["ic"] = ic;
            return View();
        }

        public unsafe ActionResult Index2()
        {
            return View();
        }

        [HttpPost]
        public unsafe ActionResult Pricer(int nbSamples)
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
                nbSamples,
                spots,
                volatilities,
                0.0,
                correlations,
                trends,
                &price,
                &ic);

            ViewData["price"] = price;
            ViewData["ic"] = ic;
            return PartialView();
        }

    }
}