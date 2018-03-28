using PEPS_Beta.Models;
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
        static extern unsafe void PriceMultimonde2021(
            int sampleNumber,
            double[] spots,
            double[] volatilities,
            double interestRate,
            double[] correlations,
            double[] trends,
            double* price,
            double* ic
        );

        [DllImport(@"..\..\x64\Debug\PricerDll.dll")]
        static extern unsafe void PriceMultimonde2021Quanto(
            int sampleNumber,
            double[] past, // format [,]
            int nbRows,
            double t,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double* price,
            double* ic
            );

        [DllImport(@"..\..\x64\Debug\PricerDll.dll")]
        static extern unsafe void DeltasMultimonde2021Quanto(
            int sampleNumber,
            double[] past, // format [,]
            int nbRows,
            double t,
            double[] currentPrices,
            double[] volatilities,
            double[] interestRates,
            double[] correlations, //6*6=36, traduction naturelle (non fortran) [ligne*6+colonne] <-> [ligne][colonne]
            double** deltas);



        // GET: Home
        public unsafe ActionResult Index()
        {
                        // ParseData


            //ds.Update();
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
                //dal.Init();

                List<Indice> indices = dal.GetIndices();
                List<TauxDeChange> tauxDC = dal.GetTDC();

                Models.DataStorage ds = new Models.DataStorage();
                //ds.FillDataHtml(500, 500);
                //ds.FillDataHtml();
                //ds.DataToArray();
                double[,] dataAssets = new double[1,1];//= ds.IndexValues;
                double[,] dataFX = new double[1,1];//= ds.ChangeValues;


                /*for (int k = 0; k < dataAssets.GetLength(1); k++)
                {
                    for (int i=0; i < dataAssets.GetLength(0); i++)
                    {
                        if(dataAssets[i,k]
                    }
                }*/

                int optionSize = dataAssets.GetLength(0) + dataFX.GetLength(0);
                double[,] data_ = new double[optionSize, dataAssets.GetLength(1)];
                /*int k = 0;
                foreach (Indice j in indices)
                {
                    double[] dataJ = new double[j.Histo.Count];
                    j.Histo.Values.CopyTo(dataJ, k);
                    k++;
                    for (int x = 0; x < dataJ.Length; x++)
                    {
                        data_[k, x] = dataJ[x];
                    }
                }*/
                for (int i=0; i < data_.GetLength(0); i++)
                {
                    for (int x = 0; x < data_.GetLength(1); x++)
                    {
                        if(i < dataAssets.GetLength(0))
                        {
                            data_[i, x] = dataAssets[i, x];
                        }
                        else
                        {
                            data_[i, x] = dataFX[i, x];
                        }
                    }
                }
                double[] volatilities = new double[optionSize];
                double[,] covMat = PricerDll.CustomTests.MathUtils.ComputeCovMatrix(PricerDll.CustomTests.MathUtils.ComputeReturns(data_));
                volatilities = PricerDll.CustomTests.MathUtils.ComputeVolatility(covMat);
                foreach (Indice i in indices)
                {
                    dal.modifierIndice(i.Id, i.InterestRateThisArea, volatilities[indices.IndexOf(i)]);
                    //dal.modifierIndice(i.Id, i.InterestRateThisArea, 15.0);
                    
                    //i.Vol = volatilities[indices.IndexOf(i)];
                }
                double[,] corrMat = PricerDll.CustomTests.MathUtils.ComputeCorrMatrix(covMat);
                foreach (Indice i in indices)
                {
                    foreach (Indice j in indices)
                    {
                        i.CorrelationMat.Add(j,corrMat[indices.IndexOf(i), indices.IndexOf(j)]);
                    }
                    foreach(TauxDeChange j in tauxDC)
                    {
                        //i.CorrelationMatTC.Add(j, corrMat[indices.IndexOf(i), indices.Count + tauxDC.IndexOf(j)]);
                    }
                }
                // ne pas toucher au return
                return PartialView(dal.GetIndices());
            }
        }

        public ActionResult UpdatePortefeuille()
        {
            using (DAL dal = new DAL())
            {
                DateTime currD = dal.GetParams().CurrDate;
                double portValue = 0.0;
                double optimumValue = 0.0;
                MultiMondeParam param = dal.GetParams();
                PortefeuilleIdeal IdealPort = dal.getPortOpti();

                double currTDC;
                double currP;
                double currZC;
                double Tmoinst = (param.EndDate - currD).TotalDays / 365.0;
                List<Indice> indList = dal.GetIndices();
                double tauxEuro = 0.0;
                foreach (Indice ind in indList)
                {
                    if (ind.Money == "eur")
                    {
                        tauxEuro = ind.InterestRateThisArea;
                        currTDC = 1.0;
                    }
                    else
                    {
                        currTDC = dal.getSingleChange(currD, "EUR" + ind.Money.ToUpper());
                    }
                    currP = dal.getSingleData(currD, ind.Nom.ToUpper()) / currTDC;
                    currZC = Math.Exp(-ind.InterestRateThisArea * Tmoinst) / currTDC;

                    portValue += dal.getPortefeuilleCouverture().GetDelta(ind.Nom) * currP;
                    portValue += dal.getPortefeuilleCouverture().GetDelta(ind.Money) * currZC;
                    optimumValue += IdealPort.GetDelta(ind.Nom) * currP;
                    optimumValue += IdealPort.GetDelta(ind.Money) * currZC;

                    dal.SetDelta(ind.Nom, IdealPort.GetDelta(ind.Nom));
                    dal.SetDelta(ind.Money, IdealPort.GetDelta(ind.Money));
                }

                double restant = portValue - optimumValue;
                restant *= Math.Exp(tauxEuro * Tmoinst);
                restant += IdealPort.GetDelta("eur");
                dal.SetDelta("eur", restant);

                return PartialView("Couverture", dal.getPortefeuilleCouverture());

            }
        }

        public ActionResult InitPortefeuille(CouvertureIdealeViewModel couvertureIdealeViewModel)
        {
            using (DAL dal = new DAL())
            {
                dal.SetPort(dal.getPortOpti());

                return PartialView("Couverture", dal.getPortefeuilleCouverture());
            }
        }

        public ActionResult VoirCurrPort()
        {
            using (DAL dal = new DAL())
            {
                return PartialView("Couverture", dal.getPortefeuilleCouverture());
            }
        }

        public ActionResult CalculerDeltas()
        {
            using (DAL dal = new DAL())
            {
                CouvertureIdealeViewModel vm = new CouvertureIdealeViewModel();
                vm.IdealPort = new PortefeuilleIdeal();
                vm.IdealPort.DeltaAsx = 1.0;
                vm.CurrDate = new DateTime(2015, 10, 01);
                dal.saveOpti(vm.IdealPort);
                return PartialView("CouvertureIdeale", vm);
                //return Content("Deltas en cours d'implémentation");
            }
        }
    }
}