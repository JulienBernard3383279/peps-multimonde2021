using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

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
            DateTime[] previousDT = new DateTime[6];
            DateTime[] currentDT = new DateTime[6];
            double businessDays = 0;
            // Fill indexValue with dateCheck for missing values
            // Init outside loop

            asx.Add(DateTime.Parse(DR.ASX.TimeSeries[0].DateTime).AddDays(1), Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            estox.Add(DateTime.Parse(DR.ESTOX.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            ftse.Add(DateTime.Parse(DR.FTSE.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            sp500.Add(DateTime.Parse(DR.SP500.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            n225.Add(DateTime.Parse(DR.N225.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.N225.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            hang.Add(DateTime.Parse(DR.HANG.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

            previousDT[0] = DateTime.Parse(DR.ASX.TimeSeries[0].DateTime);
            previousDT[1] = DateTime.Parse(DR.ESTOX.TimeSeries[0].DateTime);
            previousDT[2] = DateTime.Parse(DR.FTSE.TimeSeries[0].DateTime);
            previousDT[3] = DateTime.Parse(DR.SP500.TimeSeries[0].DateTime);
            previousDT[4] = DateTime.Parse(DR.N225.TimeSeries[0].DateTime);
            previousDT[5] = DateTime.Parse(DR.HANG.TimeSeries[0].DateTime);

            for (int i = 1; i < nbValueStock; i++)
            {
                ////// ASX //////
                // Add 1 day to all date because Australian + DayOfWeek.Sunday
                currentDT[0] = DateTime.Parse(DR.ASX.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[0], previousDT[0]) - 1;
                if (businessDays == 1)
                    asx.Add(DateTime.Parse(DR.ASX.TimeSeries[i].DateTime).AddDays(1), Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[0].DayOfWeek == DayOfWeek.Sunday || previousDT[0].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[0] = previousDT[0].AddDays(-3);
                            asx.Add(previousDT[0].AddDays(1), asx.Values.Last());
                        } else
                        {
                            previousDT[0] = previousDT[0].AddDays(-1);
                            asx.Add(previousDT[0].AddDays(1), asx.Values.Last());
                        }

                    }
                    asx.Add(DateTime.Parse(DR.ASX.TimeSeries[i].DateTime).AddDays(1), Convert.ToDouble(double.Parse(DR.ASX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[0] = DateTime.Parse(DR.ASX.TimeSeries[i].DateTime);

                ////// ESTOX /////
                currentDT[1] = DateTime.Parse(DR.ESTOX.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[1], previousDT[1]) - 1;
                if (businessDays == 1)
                    estox.Add(DateTime.Parse(DR.ESTOX.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[1].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[1] = previousDT[1].AddDays(-3);
                            estox.Add(previousDT[1], estox.Values.Last());
                        }
                        else
                        {
                            previousDT[1] = previousDT[1].AddDays(-1);
                            estox.Add(previousDT[1], estox.Values.Last());
                        }

                    }
                    estox.Add(DateTime.Parse(DR.ESTOX.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.ESTOX.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[1] = DateTime.Parse(DR.ESTOX.TimeSeries[i].DateTime);

                ///// FTSE //////
                currentDT[2] = DateTime.Parse(DR.FTSE.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[2], previousDT[2]) - 1;
                if (businessDays == 1)
                    ftse.Add(DateTime.Parse(DR.FTSE.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[2].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[2] = previousDT[2].AddDays(-3);
                            ftse.Add(previousDT[2], ftse.Values.Last());
                        }
                        else
                        {
                            previousDT[2] = previousDT[2].AddDays(-1);
                            ftse.Add(previousDT[2], ftse.Values.Last());
                        }

                    }
                    ftse.Add(DateTime.Parse(DR.FTSE.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.FTSE.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[2] = DateTime.Parse(DR.FTSE.TimeSeries[i].DateTime);

                ///// sp500 ////////
                currentDT[3] = DateTime.Parse(DR.SP500.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[3], previousDT[3]) - 1;
                if (businessDays == 1)
                    sp500.Add(DateTime.Parse(DR.SP500.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[3].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[3] = previousDT[3].AddDays(-3);
                            sp500.Add(previousDT[3], sp500.Values.Last());
                        }
                        else
                        {
                            previousDT[3] = previousDT[3].AddDays(-1);
                            sp500.Add(previousDT[3], sp500.Values.Last());
                        }

                    }
                    sp500.Add(DateTime.Parse(DR.SP500.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.SP500.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[3] = DateTime.Parse(DR.SP500.TimeSeries[i].DateTime);

                ///// N225 //////
                currentDT[4] = DateTime.Parse(DR.N225.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[4], previousDT[4]) - 1;
                if (businessDays == 1)
                    n225.Add(DateTime.Parse(DR.N225.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.N225.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[4].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[4] = previousDT[4].AddDays(-3);
                            n225.Add(previousDT[4], n225.Values.Last());
                        }
                        else
                        {
                            previousDT[4] = previousDT[4].AddDays(-1);
                            n225.Add(previousDT[4], n225.Values.Last());
                        }

                    }
                    n225.Add(DateTime.Parse(DR.N225.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.N225.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[4] = DateTime.Parse(DR.N225.TimeSeries[i].DateTime);
                ///// HANG /////
                currentDT[5] = DateTime.Parse(DR.HANG.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentDT[5], previousDT[5]) - 1;
                if (businessDays == 1)
                    hang.Add(DateTime.Parse(DR.HANG.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousDT[5].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousDT[5] = previousDT[5].AddDays(-3);
                            hang.Add(previousDT[5], hang.Values.Last());
                        }
                        else
                        {
                            previousDT[5] = previousDT[5].AddDays(-1);
                            hang.Add(previousDT[5], hang.Values.Last());
                        }

                    }
                    hang.Add(DateTime.Parse(DR.HANG.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.HANG.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                previousDT[5] = DateTime.Parse(DR.HANG.TimeSeries[i].DateTime);
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
            DateTime[] previousCDT = new DateTime[5];
            DateTime[] currentCDT = new DateTime[5];
            //Init fill
            tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
            EurUsd.Add(DateTime.Parse(DR.EURUSD.TimeSeries[0].DateTime), tmpEURUSD);
            tmpEURAUD = tmpEURUSD / Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
            EurAud.Add(DateTime.Parse(DR.AUDUSD.TimeSeries[0].DateTime), tmpEURAUD);
            EurGbp.Add(DateTime.Parse(DR.EURGBP.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            EurJpy.Add(DateTime.Parse(DR.EURJPY.TimeSeries[0].DateTime), Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
            tmpEURHKD = tmpEURUSD * Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[0].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
            EurHkd.Add(DateTime.Parse(DR.USDHKD.TimeSeries[0].DateTime), tmpEURHKD);

            previousCDT[0] = DateTime.Parse(DR.EURUSD.TimeSeries[0].DateTime);
            previousCDT[1] = DateTime.Parse(DR.AUDUSD.TimeSeries[0].DateTime);
            previousCDT[2] = DateTime.Parse(DR.EURGBP.TimeSeries[0].DateTime);
            previousCDT[3] = DateTime.Parse(DR.EURJPY.TimeSeries[0].DateTime);
            previousCDT[4] = DateTime.Parse(DR.USDHKD.TimeSeries[0].DateTime);

            for (int i = 1; i < nbValueExchange; i++)
            {
                //////// EURUSD ////////
                currentCDT[0] = DateTime.Parse(DR.EURUSD.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays( currentCDT[0], previousCDT[0]) -1;
                if (businessDays == 1)
                {
                    tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurUsd.Add(DateTime.Parse(DR.EURUSD.TimeSeries[i].DateTime), tmpEURUSD);
                }
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousCDT[0].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousCDT[0] = previousCDT[0].AddDays(-3);
                            EurUsd.Add(previousCDT[0], EurUsd.Values.Last());
                        }
                        else
                        {
                            previousCDT[0] = previousCDT[0].AddDays(-1);
                            EurUsd.Add(previousCDT[0], EurUsd.Values.Last());
                        }

                    }
                    tmpEURUSD = Convert.ToDouble(double.Parse(DR.EURUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurUsd.Add(DateTime.Parse(DR.EURUSD.TimeSeries[i].DateTime), tmpEURUSD);

                }
                previousCDT[0] = DateTime.Parse(DR.EURUSD.TimeSeries[i].DateTime);

                ///////// EURAUD //////////
                // EURAUD = EURUSD / AUDUSD
                
                currentCDT[1] = DateTime.Parse(DR.AUDUSD.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays( currentCDT[1], previousCDT[1]) -1;
                if (businessDays == 1)
                {
                    tmpEURAUD = tmpEURUSD / Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurAud.Add(DateTime.Parse(DR.AUDUSD.TimeSeries[i].DateTime), tmpEURAUD);
                }
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousCDT[1].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousCDT[1] = previousCDT[1].AddDays(-3);
                            EurAud.Add(previousCDT[1], EurAud.Values.Last());
                        }
                        else
                        {
                            previousCDT[1] = previousCDT[1].AddDays(-1);
                            EurAud.Add(previousCDT[1], EurAud.Values.Last());
                        }

                    }
                    tmpEURAUD = tmpEURUSD / Convert.ToDouble(double.Parse(DR.AUDUSD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurAud.Add(DateTime.Parse(DR.AUDUSD.TimeSeries[i].DateTime), tmpEURAUD);
                }
                previousCDT[1] = DateTime.Parse(DR.AUDUSD.TimeSeries[i].DateTime);

                ///////// EURGBP /////////
                currentCDT[2] = DateTime.Parse(DR.EURGBP.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentCDT[2], previousCDT[2]) -1;
                if (businessDays == 1)
                {
                    EurGbp.Add(DateTime.Parse(DR.EURGBP.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousCDT[2].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousCDT[2] = previousCDT[2].AddDays(-3);
                            EurGbp.Add(previousCDT[2], EurGbp.Values.Last());
                        }
                        else
                        {
                            previousCDT[2] = previousCDT[2].AddDays(-1);
                            EurGbp.Add(previousCDT[2], EurGbp.Values.Last());
                        }

                    }
                    EurGbp.Add(DateTime.Parse(DR.EURGBP.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURGBP.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                }
                previousCDT[2] = DateTime.Parse(DR.EURGBP.TimeSeries[i].DateTime);

                //////// EUR JPY /////////
                currentCDT[3] = DateTime.Parse(DR.EURJPY.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays(currentCDT[3], previousCDT[3]) -1;
                if (businessDays == 1)
                {
                    EurJpy.Add(DateTime.Parse(DR.EURJPY.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));
                }
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousCDT[3].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousCDT[3] = previousCDT[3].AddDays(-3);
                            EurJpy.Add(previousCDT[3], EurJpy.Values.Last());
                        }
                        else
                        {
                            previousCDT[3] = previousCDT[3].AddDays(-1);
                            EurJpy.Add(previousCDT[3], EurJpy.Values.Last());
                        }

                    }
                    EurJpy.Add(DateTime.Parse(DR.EURJPY.TimeSeries[i].DateTime), Convert.ToDouble(double.Parse(DR.EURJPY.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture)));

                }
                previousCDT[3] = DateTime.Parse(DR.EURJPY.TimeSeries[i].DateTime);

                /////// EURHKD ////////
                // EURHKD = EURUSD * USDHKD
                currentCDT[4] = DateTime.Parse(DR.USDHKD.TimeSeries[i].DateTime);
                businessDays = GetBusinessDays( currentCDT[4], previousCDT[4]) -1;
                if (businessDays == 1)
                {
                    tmpEURHKD = tmpEURUSD * Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurHkd.Add(DateTime.Parse(DR.USDHKD.TimeSeries[i].DateTime), tmpEURHKD);
                }
                else
                {
                    for (int j = 0; j < businessDays - 1; j++)
                    {
                        if (previousCDT[4].DayOfWeek == DayOfWeek.Monday)
                        {
                            previousCDT[4] = previousCDT[4].AddDays(-3);
                            EurHkd.Add(previousCDT[4], EurHkd.Values.Last());
                        }
                        else
                        {
                            previousCDT[4] = previousCDT[4].AddDays(-1);
                            EurHkd.Add(previousCDT[4], EurHkd.Values.Last());
                        }

                    }
                    tmpEURHKD = tmpEURUSD * Convert.ToDouble(double.Parse(DR.USDHKD.TimeSeries[i].adjustedclose, System.Globalization.CultureInfo.InvariantCulture));
                    EurHkd.Add(DateTime.Parse(DR.USDHKD.TimeSeries[i].DateTime), tmpEURHKD);
                }
                previousCDT[4] = DateTime.Parse(DR.USDHKD.TimeSeries[i].DateTime);  
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