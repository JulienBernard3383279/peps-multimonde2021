using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PEPS_Beta.Models;

namespace PEPS_Beta.Tests
{
    [TestClass]
    public class TestWeb
    {
        [TestMethod]
        public void TestInit()
        {
            using (DAL dal = new DAL())
            {
                List<Indice> indices = dal.GetIndices();
                Assert.IsNotNull(indices);
                Assert.AreEqual(6, indices.Count);
                Assert.AreEqual("estox", indices[0].Nom);
                MultiMondeParam param = dal.GetParams();
                Assert.AreEqual(1000, param.NbSamples);
            }
        }

        [TestMethod]
        public void NewTest()
        {
            Assert.AreEqual(1.0,0.5);
        }
    }
}
