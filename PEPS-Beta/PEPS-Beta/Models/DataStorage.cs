using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.SqlClient;

/// BUG
/// Ne renvoit pas les résultats souhaités SI le jour d'actualisation certaines données sont manquantes
/// BUG
namespace PEPS_Beta.Models
{
#pragma warning disable CS0436
    public class DataStorage
    {
        // Index Markets
        // violation regle nommage, a changer plus tard
        public Dictionary<DateTime, double> asx { get; private set; }
        public Dictionary<DateTime, double> estox { get; private set; }
        public Dictionary<DateTime, double> ftse { get; private set; }
        public Dictionary<DateTime, double> hang { get; private set; }
        public Dictionary<DateTime, double> n225 { get; private set; }
        public Dictionary<DateTime, double> sp500 { get; private set; }

        // Currencies : EUR, USD, AUD, GBP, JPY, HKD
        public Dictionary<DateTime, double> EurUsd { get; private set; }
        public Dictionary<DateTime, double> EurAud { get; private set; }
        public Dictionary<DateTime, double> EurGbp { get; private set; }
        public Dictionary<DateTime, double> EurJpy { get; private set; }
        public Dictionary<DateTime, double> EurHkd { get; private set; }
        public String LastUpdate { get; private set; }

        public DataStorage()
        {
            asx = new Dictionary<DateTime, double>();
            estox = new Dictionary<DateTime, double>();
            ftse = new Dictionary<DateTime, double>();
            hang = new Dictionary<DateTime, double>();
            n225 = new Dictionary<DateTime, double>();
            sp500 = new Dictionary<DateTime, double>();


            EurAud = new Dictionary<DateTime, double>();
            EurUsd = new Dictionary<DateTime, double>();
            EurGbp = new Dictionary<DateTime, double>();
            EurJpy = new Dictionary<DateTime, double>();
            EurHkd = new Dictionary<DateTime, double>();
            LastUpdate = "15/10/2015";
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
         * dataLength must be "compact" or "full"
         * */

        // /!\ nbValue must be inferior to a max Value which is around 4600 (represents 20 years of data )

        public void FillDataHtml(string dataLength, int nbValueStock, int nbValueExchange)
        {

            HtmlDataRetriever DR = new HtmlDataRetriever();
            DR.RetrieveData(dataLength);
            for (int i = 0; i < nbValueStock; i++)
            {
                ////// ASX //////
                asx.Add(DateTime.Parse(DR.ASX.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                ////// ESTOX /////
                estox.Add(DateTime.Parse(DR.ESTOX.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                ///// FTSE //////
                ftse.Add(DateTime.Parse(DR.FTSE.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));


                ///// sp500 ////////
                sp500.Add(DateTime.Parse(DR.SP500.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));


                ///// N225 //////
                n225.Add(DateTime.Parse(DR.N225.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.N225.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                ///// HANG /////
                hang.Add(DateTime.Parse(DR.HANG.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

            }


            // Fill ChangeValues
            /*
                * Column 1 : EUR/AUD
                * Column 2 : EUR/USD
                * Column 3 : EUR/GBP
                * Column 4 : EUR/JPY
                * Column 5 : EUR/HKD
            */
            // Additional operations needs to be done as not all change rates includes EUR
            // /!\ Compatibilité des dates pour les opérations sur les monnaies ? OSEF vu qu'en cas de manque on met la date de la veille ?
            double tmpEURUSD = 0;
            double tmpEURAUD = 0;
            double tmpEURHKD = 0;


            for (int i = 0; i < nbValueExchange; i++)
            {
                //////// EURUSD ////////

                tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                EurUsd.Add(DateTime.Parse(DR.EURUSD.TimeSeries[i].DateTime), tmpEURUSD);

                ///////// EURAUD //////////
                // EURAUD = EURUSD / AUDUSD


                ///////// EURGBP /////////
                EurGbp.Add(DateTime.Parse(DR.EURGBP.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                //////// EUR JPY /////////
                EurJpy.Add(DateTime.Parse(DR.EURJPY.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));


            }
            DateTime dateAUDUSD;
            DateTime dateUSDHKD;
            double tmpAUDUSD;
            double tmpUSDHKD;

            // 2nd loop to fill data needing an operation
            for (int i = 0; i < nbValueExchange; i++)
            {
                dateAUDUSD = DateTime.Parse(DR.AUDUSD.TimeSeries[i].DateTime);
                if (EurUsd.ContainsKey(dateAUDUSD))
                {
                    tmpAUDUSD = Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    if (tmpAUDUSD != 0)
                    {
                        tmpEURAUD = EurUsd[dateAUDUSD] / tmpAUDUSD;
                        EurAud.Add(dateAUDUSD, tmpEURAUD);
                    }
                }
                else
                {
                    EurAud.Add(dateAUDUSD, -1);
                }
                dateUSDHKD = DateTime.Parse(DR.USDHKD.TimeSeries[i].DateTime);
                if (EurUsd.ContainsKey(dateAUDUSD))
                {
                    tmpUSDHKD = Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    if (tmpUSDHKD != 0)
                    {
                        tmpEURHKD = EurUsd[dateUSDHKD] * tmpUSDHKD;
                        EurHkd.Add(dateUSDHKD, tmpEURHKD);
                    }
                }
                else
                {
                    EurHkd.Add(dateUSDHKD, -1);
                }

            }
            LastUpdate = DR.ASX.TimeSeries[0].DateTime;
        }


        /*
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
                    // si le dernier update date de plus 100jours ne rien faire 
                    if (count == 100 && dateFound == false)
                        return;

                    //Add data 

                    double tmpEURUSD = 0;
                    for (int i = 1; i < count + 1; i++)
                    {
                        // STOCK
                        this.IndexValues[0, IndexLength - i] = Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.IndexValues[1, IndexLength - i] = Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.IndexValues[2, IndexLength - i] = Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.IndexValues[3, IndexLength - i] = Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.IndexValues[4, IndexLength - i] = Convert.ToDouble(double.Parse(DR.N225.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.IndexValues[5, IndexLength - i] = Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));

                        // EXCHANGE

                        tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        // EURAUD = EURUSD / AUDUSD
                        this.ChangeValues[0, ChangeLength - i] = tmpEURUSD / Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.ChangeValues[1, ChangeLength - i] = tmpEURUSD;
                        this.ChangeValues[2, ChangeLength - i] = Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                        this.ChangeValues[3, ChangeLength - i] = Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));

                        // EURHKD = EURUSD * USDHKD
                        this.ChangeValues[4, ChangeLength + i] = tmpEURUSD * Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[100 - i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    }

                }
                */


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
        /*
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
                }*/

        public double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }
    }
#pragma warning restore CS0436

}