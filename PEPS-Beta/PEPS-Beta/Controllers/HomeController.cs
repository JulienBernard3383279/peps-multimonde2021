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
        extern static double Price(
            int optionType,
            double maturity,
            int optionSize,
            double strike,
            double[] payoffCoefficients,
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double correlation, //devra être une matrice
            int timestepCustom, //0 = basic, 1 = custom
            int timestepNumber, //osef si 1
            double[] timestepCustoms, //osef si 0
            double[] trends);

            // GET: Home
            public ActionResult Index()
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

            double d = Price(
                1, //optionType 1=Basket
                3.0, //maturity in years
                40, //optionSize
                100, //strike when applicable
                payoffCoefficients, //payoffCoefficients
                50000, //nbSamples
                spots, //spots
                volatilities, //volatilities
                0.04879, //interest rate
                0.0, //correlation
                0, //timestepCustom, 0 = basic, 1 = custom
                1, //osef if 1
                new double[] { }, //osef if 0
                trends); //trends

            ViewData["d"] = d;
            return View();
        }
    }
}