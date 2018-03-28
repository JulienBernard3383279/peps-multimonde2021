using Microsoft.VisualStudio.TestTools.UnitTesting;
using PEPS_Beta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricerDll.CustomTests;

namespace PEPS_Beta.Tests
{
    [TestClass]
    public class TestEstimParam
    {
        [TestMethod]
        public void TestEstimVol()
        {
            //IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
            //Database.SetInitializer(init);
            //init.InitializeDatabase(new BddContext());

            //using (DAL dal = new DAL())
            //{
            //    dal.Init();

            //    List<Indice> indices = dal.GetIndices();

            //    int optionSize = indices.Count;
            //    double[,] data_ = new double[optionSize, indices[0].Histo.Count];
            //    int k = 0;
            //    foreach (Indice j in indices)
            //    {
            //        double[] dataJ = new double[j.Histo.Count];
            //        j.Histo.Values.CopyTo(dataJ, k);
            //        k++;
            //        for (int x = 0; x< dataJ.Length; x++)
            //        {
            //                data_[k, x] = dataJ[x];
            //        }
            //    }
            //    double[] volatilities = new double[optionSize];
            //    double[,] covMat = PricerDll.CustomTests.MathUtils.ComputeCovMatrix(PricerDll.CustomTests.MathUtils.ComputeReturns(data_));
            //    volatilities = PricerDll.CustomTests.MathUtils.ComputeVolatility(covMat);
            //    double[] correlations = new double[optionSize * optionSize];
            //    double[,] corrMat = PricerDll.CustomTests.MathUtils.ComputeCorrMatrix(covMat);
            //    Console.WriteLine("les correlations valent :");
            //    for (int i = 0; i < optionSize; i++)
            //    {
            //        for (int j = 0; j < optionSize; j++)
            //        {
            //            correlations[i * optionSize + j] = corrMat[i, j];
            //            Console.WriteLine(correlations[i * optionSize + j]);
            //        }
            //    }
            //}
        }
    }
}
