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
            out IntPtr deltas);



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
            using (DAL dal = new DAL())
            {
                // Il faut estimer les vol des indices et les correlations
                // entre les dates estim.DebutEstim et estim.FinEstim

                List<Indice> indices = dal.GetIndices();

                double[,] dataAssets = dal.getIndexValues(estim.DebutEst, estim.FinEst);

                double[,] dataFX = dal.getChangeValues(estim.DebutEst, estim.FinEst);

                //// Indices en euros
                //for (int k = 0; k < dataAssets.GetLength(0); k++)
                //{
                //    dataAssets[k, 1] /= dataFX[k, 0];
                //    dataAssets[k, 2] /= dataFX[k, 1];
                //    dataAssets[k, 3] /= dataFX[k, 2];
                //    dataAssets[k, 4] /= dataFX[k, 3];
                //    dataAssets[k, 5] /= dataFX[k, 4];
                //}

                int optionSize = dataAssets.GetLength(1) + dataFX.GetLength(1);
                double[,] data_ = new double[dataAssets.GetLength(0), optionSize];

                for (int row = 0; row < data_.GetLength(0); row++)
                {
                    for (int x = 0; x < data_.GetLength(1); x++)
                    {
                        if (x < dataAssets.GetLength(1))
                        {
                            data_[row, x] = dataAssets[row, x];
                        }
                        else
                        {
                            data_[row, x] = dataFX[row, x - dataAssets.GetLength(1)];
                        }
                    }
                }

                double[] volatilities = new double[optionSize];
                double[,] covMat = PricerDll.CustomTests.MathUtils.ComputeCovMatrix(PricerDll.CustomTests.MathUtils.ComputeReturns(data_));
                volatilities = PricerDll.CustomTests.MathUtils.ComputeVolatility(covMat);

                foreach (Indice i in indices)
                {
                    dal.modifierIndice(i.Id, i.InterestRateThisArea, volatilities[indices.IndexOf(i)]);

                    //i.Vol = volatilities[indices.IndexOf(i)];
                }
                double[,] corrMat = PricerDll.CustomTests.MathUtils.ComputeCorrMatrix(covMat);
                foreach (Indice i in indices)
                {
                    int indexI = indices.IndexOf(i);
                    dal.setCorrIndice(i.Id, 
                        corrMat[indexI, 0],
                        corrMat[indexI, 1],
                        corrMat[indexI, 2],
                        corrMat[indexI, 3],
                        corrMat[indexI, 4],
                        corrMat[indexI, 5],
                        corrMat[indexI, 6],
                        corrMat[indexI, 7],
                        corrMat[indexI, 8],
                        corrMat[indexI, 9],
                        corrMat[indexI, 10]
                        );



                    //int indexI = indices.IndexOf(i);

                    //foreach (Indice j in indices)
                    //{
                    //    double t = corrMat[indexI, indices.IndexOf(j)];
                    //    i.CorrelationMat.Add(j, t);
                    //}

                    //i.CorrelationMatTC.Add("EURUSD", corrMat[indexI, indices.Count + 0]);
                    //i.CorrelationMatTC.Add("EURJPY", corrMat[indexI, indices.Count + 1]);
                    //i.CorrelationMatTC.Add("EURHKD", corrMat[indexI, indices.Count + 2]);
                    //i.CorrelationMatTC.Add("EURGBP", corrMat[indexI, indices.Count + 3]);
                    //i.CorrelationMatTC.Add("EURAUX", corrMat[indexI, indices.Count + 4]);


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
                double Tmoinst = (param.EndDate - currD).TotalDays / 365.25;
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

        public unsafe ActionResult CalculerDeltas()
        {
            using (DAL dal = new DAL())
            {
                CouvertureIdealeViewModel vm = new CouvertureIdealeViewModel();
                vm.IdealPort = new PortefeuilleIdeal();

                MultiMondeParam param = dal.GetParams();
                double t = (param.CurrDate-param.Origin).TotalDays / 365.25;
                double currTDC;
                double currP;
                DateTime currD = param.CurrDate;
                List<Indice> indList = dal.GetIndices();
                int i = 0;
                double[] currentPrices = new double[11];
                double[] volatilities = new double[11];
                double[] interestRates = new double[6];
                double[] correlations = new double[11 * 11];
                foreach (Indice ind in indList)
                {
                    if (ind.Money != "eur")
                    {
                        currTDC = dal.getSingleChange(currD, "EUR" + ind.Money.ToUpper());
                        //currTDC = 0.5;
                        currentPrices[i + 5] = currTDC;
                        volatilities[i + 5] = 0.02;
                        correlations[(i + 5) * (i + 5)] = 1.0;
                    }
                    currP = dal.getSingleData(currD, ind.Nom.ToUpper());
                    //currP = 100;
                    currentPrices[i] = currP;
                    volatilities[i] = ind.Vol;
                    interestRates[i] = ind.InterestRateThisArea;
                    correlations[i*i] = 1.0;
                    i++;
                }

                int nbRows = 1;

                foreach (DateTime constat in param.Constatations)
                {
                    if (DateTime.Compare(constat,currD)<=0)
                    {
                        nbRows += 1;
                    }
                }
                double[] past = new double[11 * nbRows];



                int row = 0;
                i = 0;
                foreach (Indice ind in indList)
                {
                    if (ind.Money != "eur")
                    {
                        currTDC = dal.getSingleChange(param.Origin, "EUR" + ind.Money.ToUpper());
                        //currTDC = 100;
                        past[11 * row + i + 5] = currTDC;
                    }
                    currP = dal.getSingleData(param.Origin, ind.Nom.ToUpper());
                    //currP = 100;
                    past[11 * row + i] = currP;
                    i++;
                }
                foreach (DateTime constat in param.Constatations)
                {
                    row += 1;
                    if (DateTime.Compare(constat, currD) <= 0)
                    {
                        i = 0;
                        foreach (Indice ind in indList)
                        {
                            if (ind.Money != "eur")
                            {
                                currTDC = dal.getSingleChange(constat, "EUR" + ind.Money.ToUpper());
                                //currTDC = 100;
                                past[11 * row + i + 5] = currTDC;
                            }
                            currP = dal.getSingleData(constat, ind.Nom.ToUpper());
                            //currP = 100;
                            past[11 * row + i] = currP;
                            i++;
                        }
                    }
                }

                IntPtr deltasPtr;

                DeltasMultimonde2021Quanto(param.NbSamples, past, nbRows, t, currentPrices, volatilities, interestRates, correlations, out deltasPtr);

                double[] deltas = new double[11];
                System.Runtime.InteropServices.Marshal.Copy(deltasPtr, deltas, 0, 11); //<- deltas contient maintenant les deltas

                String[] names = new String[11] {
                "estoxx",
                "sp500",
                "n225",
                "hang",
                "ftse",
                "asx",
                "usd",
                "jpy",
                "hkd",
                "gbp",
                "aud"};

                for (int k = 0; k<11; ++k)
                {
                    vm.IdealPort.SetDelta(names[k], deltas[k]);
                }

                double tauxEuro = 0;
                double currZC;
                double optimumValue = 0;
                double Tmoinst = (param.EndDate - currD).TotalDays / 365.25;

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
                        //currTDC = 0.5;
                    }
                    currP = dal.getSingleData(currD, ind.Nom.ToUpper()) / currTDC;
                    //currP = 100/currTDC;
                    currZC = Math.Exp(-ind.InterestRateThisArea * Tmoinst) / currTDC;

                    optimumValue += vm.IdealPort.GetDelta(ind.Nom) * currP;
                    optimumValue += vm.IdealPort.GetDelta(ind.Money) * currZC;
                }
                double price;
                double ic;
                PriceMultimonde2021Quanto(param.NbSamples, past, nbRows, t, currentPrices, volatilities, interestRates, correlations, &price, &ic);
                double restant = price - optimumValue;
                restant *= Math.Exp(tauxEuro * Tmoinst);
                restant += vm.IdealPort.GetDelta("eur");
                vm.IdealPort.SetDelta("eur", restant);
                dal.saveOpti(vm.IdealPort);

                return PartialView("CouvertureIdeale", vm);

                //return Content("Deltas en cours d'implémentation");
            }
        }

        public ActionResult SeeMMParam()
        {
            using (DAL dal = new DAL())
            {
                return PartialView("MultiMondeParam", dal.GetParams());
            }
        }

        public void SetDate(MultiMondeParam m)
        {
            using (DAL dal = new DAL())
            {
                dal.ChangeDate(m.CurrDate);
            }
        }
        
    }
}