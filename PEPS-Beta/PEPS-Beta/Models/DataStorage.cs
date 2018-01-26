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
        double[,] _indexValues;

        public double[,] IndexValues
        {
            get { return _indexValues; }
            private set { _indexValues = value; }
        }

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
            asx = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ASX.csv"),0);
            // CAC 40 //
            // uncomment if needed
            //cac = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\CAC40.csv"),0);

            estox = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ESTOXX50.csv"),0);
            ftse = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\FTSE100.csv"),1);
            sp500 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\SP500.csv"),0);
            n225 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\n225.csv"),0);

            hang = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\HANG.csv"),0);

        }

        /**@brief : store dictonaries values in an array 
         * Column 1 : ASX
         * Column 2 : ESTOXX
         * Column 3 : FTSE
         * Column 4 : SP500
         * Column 5 : N225
         * Column 6 : Hang
         * */

        public void DataToArray()
        {
            DateTime key;
            this.IndexValues = new double[6, asx.Count];
            int count = 0;
            foreach (KeyValuePair<DateTime, double> kvp in asx)
            {
                key = kvp.Key;
                this.IndexValues[0, count] = kvp.Value;
                this.IndexValues[1, count] = estox[key];
                this.IndexValues[2, count] = ftse[key];
                this.IndexValues[3, count] = sp500[key];
                this.IndexValues[4, count] = n225[key];
                this.IndexValues[5, count] = hang[key];
                count++;
            }

            // empty dictionnaries to save memory
            asx.Clear();
            estox.Clear();
            ftse.Clear();
            sp500.Clear();
            n225.Clear();
            hang.Clear();
        }
    }
}