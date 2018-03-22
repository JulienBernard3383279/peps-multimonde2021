using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PEPS_Beta.Tests
{
    /// <summary>
    /// Description résumée pour testBusinessDays
    /// </summary>
    [TestClass]
    public class testBusinessDays
    {
        public testBusinessDays()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active, ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        //
        // Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        // Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test de la classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilisez ClassCleanup pour exécuter du code une fois que tous les tests d'une classe ont été exécutés
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            Models.DataStorage ds = new Models.DataStorage();
            String d1 = "17-11-2018";
            String d2 = "30-12-2018";
            DateTime date1 = DateTime.ParseExact(d1, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(d2, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            Assert.IsTrue(ds.GetBusinessDays(date1, date2)==30);
            
        }
    }
}
