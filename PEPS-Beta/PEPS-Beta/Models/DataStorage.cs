using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace PEPS_Beta.Models
{
    public class DataStorage
    {
        private Dictionary<DateTime,double> asx;
        //private Dictionary<DateTime, double> cac;
        private Dictionary<DateTime, double> estox;
        private Dictionary<DateTime, double> ftse;
        private Dictionary<DateTime, double> hang;
        private Dictionary<DateTime, double> n225;
        private Dictionary<DateTime, double> sp500;

        public DataStorage()
        {
            asx = new Dictionary<DateTime, double>();
            //cac = new Dictionary<DateTime, double>();
            estox = new Dictionary<DateTime, double>();
            ftse = new Dictionary<DateTime, double>();
            hang = new Dictionary<DateTime, double>();
            n225 = new Dictionary<DateTime, double>();
            sp500 = new Dictionary<DateTime, double>();
        }

        public void FillData()
        {
            

            DataRetriever dr = new DataRetriever();
            // ASX //
            asx = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ASX.csv"),0);

            // CAC 40 //
            // uncomment if needed
            //cac = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\CAC40.csv"),0);
            //ESTOXX50//
            estox = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ESTOXX50.csv"),0);
            //FTSE100//
            ftse = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\FTSE100.csv"),1);
            //HANG//
            //N225//
            //SP500//
            sp500 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\SP500.csv"),0);
            n225 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\n225.csv"),0);

            hang = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\HANG.csv"),0);

        }

        // Now remove the dates without data



    }
}