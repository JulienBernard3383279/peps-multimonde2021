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
            TestsCall.PerformPriceTests();
            //TestsCall.PerformPriceTestsAnyTime();
            //TestsCall.PerformDeltaTests0();
            //TestsCall.PerformDeltaTestsAnyTime();

            Console.WriteLine("");
            Console.WriteLine("Test du prix d'un Quanto en 0 : ");
            Console.WriteLine("");
            TestsQuanto.PerformPriceTests();
            Console.WriteLine("");

            //TestHistoricalData.TestHisto();

            Console.WriteLine("Appuyez sur une touche pour terminer le programme.");
            Console.ReadKey();
        }
    }
}
