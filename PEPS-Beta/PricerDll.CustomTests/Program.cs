using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestsCall.PerformPriceTests();
            //TestsCall.PerformPriceTestsAnyTime();
            //TestsCall.PerformDeltaTests0();
            //TestsCall.PerformDeltaTestsAnyTime();

            //TestsQuanto.PerformPriceTests();
            //TestsQuanto.PerformDeltaTests0();
            //TestsQuanto.PerformDeltaTestsAnyTime();

            //TestHistoricalData.TestHisto();

            //TestsMultimonde2021Quanto.PerformPriceTests();
            //TestsMultimonde2021Quanto.PerformDeltaTest();
            TestsMultimonde2021Quanto.PerformTrackingErrorTest();

            //TestsSingleMonde.PerformPriceSingleMondeTests();


            Console.WriteLine("Appuyez sur une touche pour terminer le programme.");
            Console.ReadKey();
        }
    }
}
