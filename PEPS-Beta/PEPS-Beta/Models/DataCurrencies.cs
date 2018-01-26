using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class DataCurrencies
    {
        // Currencies : EUR, USD, AUD, GBP, JPY, HKD
        private Dictionary<DateTime, double> EurUsd;
        private Dictionary<DateTime, double> EurAud;
        private Dictionary<DateTime, double> EurGbp;
        private Dictionary<DateTime, double> EurJpy;
        private Dictionary<DateTime, double> EurHkd;
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
            EurAud = ds.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\Currencies\EURAUD.csv"), 2,2);
            EurUsd = ds.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\Currencies\EURUSD.csv"), 2,2);
            EurGbp = ds.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\Currencies\EURGBP.csv"), 2,2);
            EurJpy = ds.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\Currencies\EURJPY.csv"), 2,2);
            EurHkd = ds.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\Currencies\EURHKD.csv"), 2,2);
        }
    }


}