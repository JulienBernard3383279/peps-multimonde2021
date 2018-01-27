using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PricerDll.CustomTests
{
    public class DataCurrencies
    {
        // Currencies : EUR, USD, AUD, GBP, JPY, HKD
        private Dictionary<DateTime, double> EurUsd;
        private Dictionary<DateTime, double> EurAud;
        private Dictionary<DateTime, double> EurGbp;
        private Dictionary<DateTime, double> EurJpy;
        private Dictionary<DateTime, double> EurHkd;

        double[,] _changeValues;

        public double[,] ChangeValues
        {
            get { return _changeValues; }
            private set { _changeValues = value; }
        }
        public DataCurrencies()
        {
            EurAud = new Dictionary<DateTime, double>();
            EurUsd= new Dictionary<DateTime, double>();
            EurGbp =new Dictionary<DateTime, double>(); 
            EurJpy = new Dictionary<DateTime, double>();
            EurHkd = new Dictionary<DateTime, double>();
        }

        public void Fill()
        {
            DataRetriever ds = new DataRetriever();
            EurAud = ds.ReadCSVData(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\..\DataFiYa\Currencies\EURAUD.csv"), 2,2);
            EurUsd = ds.ReadCSVData(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\..\DataFiYa\Currencies\EURUSD.csv"), 2,2);
            EurGbp = ds.ReadCSVData(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\..\DataFiYa\Currencies\EURGBP.csv"), 2,2);
            EurJpy = ds.ReadCSVData(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\..\DataFiYa\Currencies\EURJPY.csv"), 2,2);
            EurHkd = ds.ReadCSVData(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\..\DataFiYa\Currencies\EURHKD.csv"), 2,2);
        }

        /**@brief :
         * Column 1 : EUR/AUD
         * Column 2 : EUR/USD
         * Column 3 : EUR/GBP
         * Column 4 : EUR/JPY
         * Column 5 : EUR/HKD
         * */
        public void DataToArray()
        {
            DateTime key;
            this.ChangeValues = new double[5, EurAud.Count];
            int count = 0;
            foreach (KeyValuePair<DateTime, double> kvp in EurAud)
            {
                key = kvp.Key;
                this.ChangeValues[0, count] = kvp.Value;
                this.ChangeValues[1, count] = EurUsd[key];
                this.ChangeValues[2, count] = EurGbp[key];
                this.ChangeValues[3, count] = EurJpy[key];
                this.ChangeValues[4, count] = EurHkd[key];
                count++;
            }

            // empty dictionnaries to save memory
            EurAud.Clear();
            EurGbp.Clear();
            EurUsd.Clear();
            EurHkd.Clear();
            EurJpy.Clear();
        }
    }


}