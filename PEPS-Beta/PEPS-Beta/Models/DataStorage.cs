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
        double[,] _changeValues;
        String LastUpdate;

        public double[,] ChangeValues
        {
            get { return _changeValues; }
            private set { _changeValues = value; }
        }

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
        /**
         * 
         * 
         * 
         * 
         * 
         *          FOR ONLINE REQUEST
         * 
         * 
         * 
         * 
         * 
         * */


        /**@brief : Fill DataStorage object with values
         * @param : nbValue indicates the number of values requested
         * 
         * */

        // /!\ nbValue must be inferior to a max Value which is around 4600 (represents 20 years of data )

        public void FillDataHtml(int nbValueStock, int nbValueExchange)
        {
            HtmlDataRetriever DR = new HtmlDataRetriever();
            DR.RetrieveData("full");

            // Fill indexValue
            this.IndexValues = new double[6, nbValueStock];
            for (int i = 0; i < nbValueStock; i++)
            {
                this.IndexValues[0, i] = Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.IndexValues[1, i] = Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.IndexValues[2, i] = Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.IndexValues[3, i] = Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.IndexValues[4, i] = Convert.ToDouble(double.Parse(DR.N225.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.IndexValues[5, i] = Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
            }

            // Fill ChangeValues
            this.ChangeValues = new double[5, nbValueExchange];
            /*
                * Column 1 : EUR/AUD
                * Column 2 : EUR/USD
                * Column 3 : EUR/GBP
                * Column 4 : EUR/JPY
                * Column 5 : EUR/HKD
            */
            // Additional operations needs to be done as not all change rates includes EUR
            double tmpEURUSD = 0;
            for (int i = 0; i<nbValueExchange;i++)
            {
                tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                // EURAUD = EURUSD / AUDUSD
                this.ChangeValues[0, i] = tmpEURUSD / Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.ChangeValues[1, i] = tmpEURUSD;
                this.ChangeValues[2, i] = Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                this.ChangeValues[3, i] = Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));

                // EURHKD = EURUSD * USDHKD
                this.ChangeValues[4,i] = tmpEURUSD * Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
            }

            LastUpdate = DR.ASX.TimeSeries[0].DateTime;

        }

        // if more than 100 days separe 2 updates full data needs to be reloaded
        public void Update()
        {
            HtmlDataRetriever DR = new HtmlDataRetriever();
            DR.RetrieveData("compact"); // load 100 last data
            //Determine amount of data to add
            //Can be optimized
            int count = 0;
            bool dateFound = false;
            while (count < 100 && dateFound == false)
            {
                dateFound = (String.Compare(LastUpdate, DR.ASX.TimeSeries[count].DateTime) == 0);
                count++;
            }
            double[,] tmpStock = new double[6, this.IndexValues.GetLength(1)];
            double[,] tmpExchange = new double[5, this.ChangeValues.GetLength(1)];
            Array.Copy(this.IndexValues, tmpStock, this.IndexValues.Length);
            Array.Copy(this.ChangeValues, tmpExchange, this.ChangeValues.Length);

            //TOEND
        }
/**
 * 
 * 
 * 
 * 
 * 
 *  FOR CSV FILES
 * 
 * 
 * 
 * 
 * 
 * */
        public void FillData()
        {


            DataRetriever dr = new DataRetriever();
            asx = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ASX.csv"), 0, 5);
            // CAC 40 //
            // uncomment if needed
            //cac = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\CAC40.csv"),0);

            estox = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\ESTOXX50.csv"), 0, 5);
            ftse = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\FTSE100.csv"), 1, 5);
            sp500 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\SP500.csv"), 0, 5);
            n225 = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\n225.csv"), 0, 5);

            hang = dr.ReadCSVData(Path.Combine(HttpRuntime.AppDomainAppPath, @"..\DataFiYa\HANG.csv"), 0, 5);

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