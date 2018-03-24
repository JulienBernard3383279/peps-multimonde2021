﻿using PEPS_Beta.Models;
using PEPS_Beta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using PricerDll.CustomTests;


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
         //   Models.DataStorage ds = new Models.DataStorage();
         //   ds.FillDataHtml(500,500);
            //

            /*
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
            ViewData["ic"] = ic;*/


            return View();
        }

        public unsafe ActionResult Index2()
        {
            return View();
        }

        [HttpPost]
        public unsafe ActionResult Pricer(int nbSamples)
        {
            int optionSize = 6;
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
        public unsafe ActionResult VoirIndicesParam()
        {
            using (DAL dal = new DAL())
            {
                return PartialView(dal.GetIndices());
            }
            // IL FAUT ENSUITE CREER CETTE VUE
            // RAJOUTER UN CONTROLEUR POUR MODIFIER LES PARAMS DUN INDICE, FAIRE AVEC BINDING DE MODELE


            // IDEM POUR PARAMS MULTIMONDE
        }

        [HttpPost]
        public unsafe void IndiceLigne(Indice ourInd)
        {
            using (DAL dal = new DAL())
            {
                dal.modifierIndice(ourInd.Id, ourInd.InterestRateThisArea, ourInd.Vol);
            }
        }

        public unsafe ActionResult EstimerParam(EstimationViewModel estim)
        {
            using(DAL dal = new DAL())
            {
                // Il faut estimer les vol des indices et les correlations
                // entre les dates estim.DebutEstim et estim.FinEstim
                dal.Init();

                List<Indice> indices = dal.GetIndices();
                //List<TauxDeChange> TauxDC = dal.GetTDC();

                int optionSize = indices.Count;
                double[,] data_ = new double[optionSize, indices[0].Histo.Count];
                int k = 0;
                foreach (Indice j in indices)
                {
                    double[] dataJ = new double[j.Histo.Count];
                    j.Histo.Values.CopyTo(dataJ, k);
                    k++;
                    for (int x = 0; x < dataJ.Length; x++)
                    {
                        data_[k, x] = dataJ[x];
                    }
                }
                double[] volatilities = new double[optionSize];
                double[,] covMat = PricerDll.CustomTests.MathUtils.ComputeCovMatrix(PricerDll.CustomTests.MathUtils.ComputeReturns(data_));
                volatilities = PricerDll.CustomTests.MathUtils.ComputeVolatility(covMat);
                foreach (Indice i in indices)
                {
                    i.Vol = volatilities[indices.IndexOf(i)];
                }
                double[,] corrMat = PricerDll.CustomTests.MathUtils.ComputeCorrMatrix(covMat);
                foreach (Indice i in indices)
                {
                    foreach (Indice j in indices)
                    {
                        i.CorrelationMat.Add(j,corrMat[indices.IndexOf(i), indices.IndexOf(j)]);
                    }
                }
                // ne pas toucher au return
                return PartialView(dal.GetIndices());
            }
        }
    }
}