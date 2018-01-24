using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace PEPS_Beta.Controllers
{
    public class HomeController : Controller
    {
        [DllImport(@"D:\Documents\ensimag\3A-Cours\PEPS\PEPS\PEPS-Beta\x64\Debug\PricerDll.dll")]
      //  [HandleProcessCorruptedStateExceptions]
        extern static double PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double correlation,
            double[] trends
        );

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

            double d = PriceMultimonde2021(
                100000,
                spots,
                volatilities,
                0.0,
                0.0,
                trends);

         
            ViewData["d"] = d;
            return View();
        }
    }
}