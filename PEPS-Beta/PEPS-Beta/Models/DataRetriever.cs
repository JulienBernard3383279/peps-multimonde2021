using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PEPS_Beta.Models
{
    // This class needs to be REFACTORED
    public class DataRetriever
    {


        public DataRetriever()
        {
        }

        /**
         * @brief :
         * Prise du prix à la date de clôture : 5 ème colonne dans les fichiers de données
         * Complétion des trous.
         * @param:
         * type depends on the dateTime format due to different sources for data
         * 0 : Yahoo
         **/
        public Dictionary<DateTime, double> ReadCSVData(String path, int type)
        {
            string line;
            DateTime compareDate;

            KeyValuePair<DateTime, Double> tmp;
            int count = 0;
            Dictionary<DateTime, double> data = new Dictionary<DateTime, double>();
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            //Skip first line (header)
            file.ReadLine();

            //Managing first line out of loop to prevent error when trying to reach last element
            //First line is supposed to have correct values
            line = file.ReadLine();
            tmp = ParseLine(line, type);
            data.Add(tmp.Key, tmp.Value);

            //Loop on the other lines
            while ((line = file.ReadLine()) != null)
            {
                tmp = ParseLine(line,type);
                //si valeur de ligne null on met le dernier prix relevé
                if (tmp.Value.CompareTo(-1.0) == 0)
                {
                    //Check if a date is missing.
                    compareDate = data.Last().Key;
                    compareDate = AddBusinessDays(compareDate, 1);
                    while (compareDate.CompareTo(tmp.Key) != 0)
                    {
                        data.Add(compareDate, data.Last().Value);
                        compareDate = AddBusinessDays(compareDate, 1);
                    }
                    data.Add(tmp.Key, data.Last().Value);
                }
                else
                {

                    //Check if a date is missing.
                    compareDate = data.Last().Key;
                    compareDate = AddBusinessDays(compareDate, 1);
                    while (compareDate.CompareTo(tmp.Key) != 0)
                    {
                        data.Add(compareDate, data.Last().Value);
                        compareDate = AddBusinessDays(compareDate, 1);
                    }
                    data.Add(tmp.Key, tmp.Value);
                }
            }
            file.Close();
            return data;
        }

        /**
         * Parse line 
         * @param:
         * type depends on the dateTime format due to different sources for data
         **/
        public KeyValuePair<DateTime, double> ParseLine(String line, int type)
        {
            // count number of comas      
            int comas = 0;
            StringBuilder kvpValues = new StringBuilder();
            DateTime date;
            IEnumerator<char> ite = line.GetEnumerator();
            //Start iteration
            ite.MoveNext();
            while (ite.Current != ',')
            {
                kvpValues.Append(ite.Current);
                ite.MoveNext();
            }
            //Parse Date : 2 possibilities atm
            if (type == 0)
                date = DateTime.ParseExact(kvpValues.ToString(), "yyyy-MM-dd", null);
            else 
                date = DateTime.ParseExact(kvpValues.ToString(), "yyyyMMdd", null);

            // Clear stringbuilder
            kvpValues.Clear();
            while (ite != null)
            {
                if (ite.Current.Equals(','))
                {
                    comas++;
                }
                if (comas == 4)
                {
                    ite.MoveNext();
                    while (!ite.Current.Equals(','))
                    {
                        kvpValues.Append(ite.Current);
                        ite.MoveNext();
                    }
                    break;
                }
                ite.MoveNext();
            }
            // try/catch because of null value appearing in csv files
            try
            {
                KeyValuePair<DateTime, double> kvpData = new KeyValuePair<DateTime, double>(date, Convert.ToDouble(kvpValues.ToString(), System.Globalization.CultureInfo.InvariantCulture));
                return kvpData;
            }
            catch (System.FormatException)
            {
                // -1 means null was read (or else but first case should represent 99% )
                KeyValuePair<DateTime, double> kvpData = new KeyValuePair<DateTime, double>(date, -1);
                return kvpData;
            }
        }

        // AddBusinessDays : used to deal with missing dates
        // Taken from : https://stackoverflow.com/questions/1044688/addbusinessdays-and-getbusinessdays
        // Untested
        public DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }
    }
}