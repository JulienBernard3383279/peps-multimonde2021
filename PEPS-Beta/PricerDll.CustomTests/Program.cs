﻿using System;
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
            TestsCall.PerformPriceTestsAnyTime();
            TestsCall.PerformDeltaTests0();
            TestsCall.PerformDeltaTestsAnyTime();

            //TestsQuanto.PerformPriceTests();
            //TestsQuanto.PerformDeltaTests0();
            //TestsQuanto.PerformDeltaTestsAnyTime();

            Console.WriteLine("Appuyez sur une touche pour terminer le programme.");
            Console.ReadKey();
        }
    }
}
