﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class DAL : IDAL
    {
        private BddContext bdd;

        public DAL()
        {
            bdd = new BddContext();
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Indice> GetIndices()
        {
            return bdd.Indices.ToList();
        }

        public MultiMondeParam GetParams()
        {
            return bdd.Parametres.ToList()[0];
        }

        public List<TauxDeChange> GetTDC()
        {
            return bdd.GetTaux.ToList();
        }

        public void Init()
        {
            if (bdd.Indices.Count() == 0)
            {
                MultiMondeParam newParam = new MultiMondeParam();
                bdd.Parametres.Add(newParam);
                Indice estox = new Indice("estox", "eur",0);
                bdd.Indices.Add(estox);
                newParam.Indices.Add(estox);
                Indice sp500 = new Indice("sp500", "usd",0);
                bdd.Indices.Add(sp500);
                newParam.Indices.Add(sp500);
                Indice n225 = new Indice("n225", "jpy",0);
                bdd.Indices.Add(n225);
                newParam.Indices.Add(n225);
                Indice hang = new Indice("hang", "hkd",0);
                bdd.Indices.Add(hang);
                newParam.Indices.Add(hang);
                Indice ftse = new Indice("ftse", "gbp",0);
                bdd.Indices.Add(ftse);
                newParam.Indices.Add(ftse);
                Indice asx = new Indice("asx", "aud",0);
                bdd.Indices.Add(asx);
                newParam.Indices.Add(asx);

                newParam.NbIndices = 6;
                newParam.NbSamples = 1000;

                bdd.SaveChanges();
            }
        }

        internal void modifierIndice(int id, double interestRateThisArea, double vol)
        {
            Indice indAmodifier = bdd.Indices.FirstOrDefault(indice => indice.Id == id);
            if (indAmodifier != null)
            {
                indAmodifier.InterestRateThisArea = interestRateThisArea;
                indAmodifier.Vol = vol;
                bdd.SaveChanges();
            }
        }

        internal void modifierIndiceVolTDC(int id, double vol)
        {
            Indice indAmodifier = bdd.Indices.FirstOrDefault(indice => indice.Id == id);
            if (indAmodifier != null)
            {
                indAmodifier.MoneyVol = vol;
                bdd.SaveChanges();
            }
        }

        internal void setCorrIndice(int id, double corrEstoxx, double corrSP500, double corrN225, double corrHANG, double corrFTSE, double corrASX, double corrEURUSD, double corrEURJPY, double corrEURHKD, double corrEURGBP, double corrEURAUD)
        {
            Indice indAmodifier = bdd.Indices.FirstOrDefault(indice => indice.Id == id);
            if (indAmodifier != null)
            {
                indAmodifier.corrESTOXX = corrEstoxx;
                indAmodifier.corrSP500 = corrSP500;
                indAmodifier.corrN225 = corrN225;
                indAmodifier.corrHANG = corrHANG;
                indAmodifier.corrFTSE = corrFTSE;
                indAmodifier.corrASX = corrASX;

                indAmodifier.corrEURUSD = corrEURUSD;
                indAmodifier.corrEURJPY = corrEURJPY;
                indAmodifier.corrEURHKD = corrEURHKD;
                indAmodifier.corrEURGBP = corrEURGBP;
                indAmodifier.corrEURAUD = corrEURAUD;

                bdd.SaveChanges();
            }
        }

        internal void setMoneyVol(int id, double moneyVol)
        {
            Indice indAmodifier = bdd.Indices.FirstOrDefault(indice => indice.Id == id);
            if (indAmodifier != null)
            {
                indAmodifier.MoneyVol = moneyVol;
                bdd.SaveChanges();
            }
        }

        public PortefeuilleCouverture getPortefeuilleCouverture()
        {
            return bdd.GetPort.ToList()[0];
        }

        public PortefeuilleIdeal getPortOpti()
        {
            return bdd.GetOptim.FirstOrDefault();
        }

        public void saveOpti(PortefeuilleIdeal port)
        {
            PortefeuilleIdeal inBdd = bdd.GetOptim.FirstOrDefault();
            if (inBdd == null)
            {
                bdd.GetOptim.Add(port);
            }
            else
            {
                inBdd.Copy(port);
            }
            bdd.SaveChanges();
        }

        public void SetDelta(String toSet, double val)
        {
            bdd.GetPort.FirstOrDefault().SetDelta(toSet, val);
            bdd.SaveChanges();
        }

        // Possible indexName ASX, ESTOX, SP500, N225, FTSE, HANG
        public double getSingleData(DateTime date, String indexName)
        {
            DateTime tmpDate = date;
            int maxTry = 0;
            if (indexName.Equals("ASX") || indexName.Equals("ESTOXX") || indexName.Equals("SP500") || indexName.Equals("N225") || indexName.Equals("HANG") || indexName.Equals("FTSE"))
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT " + indexName + " FROM  IndexesAtDates WHERE Date = @DateValue", conn);
                    command.Parameters.Add(new SqlParameter("DateValue", date));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows != false)
                        {
                            reader.Read();
                            if ((double)reader[0] != -1)
                                return (double)reader[0];
                        }
                    }
                    while (maxTry < 10)
                    {
                        tmpDate = tmpDate.AddDays(-1);
                        command = new SqlCommand("SELECT " + indexName + " FROM  IndexesAtDates WHERE Date = @DateValue", conn);
                        command.Parameters.Add(new SqlParameter("DateValue", tmpDate));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows != false)
                            {
                                reader.Read();
                                if ((double)reader[0] != -1)
                                    return (double)reader[0];
                            }
                        }
                        maxTry++;
                    }
                }
            }
            return -1;
        }

        internal void SetPort(PortefeuilleIdeal idealPort)
        {
            bdd.GetPort.FirstOrDefault().Copy(idealPort);
            bdd.SaveChanges();
        }

        public double getSingleChange(DateTime date, String changeName)
        {
            DateTime tmpDate = date;
            int maxTry = 0;
            if (changeName.Equals("EURUSD") || changeName.Equals("EURAUD") || changeName.Equals("EURGBP") || changeName.Equals("EURJPY") || changeName.Equals("EURHKD"))
            { 
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT " + changeName + " FROM  TauxDeChanges WHERE Date = @DateValue", conn);
                    command.Parameters.Add(new SqlParameter("DateValue", date));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows != false)
                        {
                            reader.Read();
                            if ((double)reader[0] != -1)
                                return (double)reader[0];
                        }
                    }
                    while (maxTry < 10)
                    {
                        tmpDate = tmpDate.AddDays(-1);
                        command = new SqlCommand("SELECT " + changeName + " FROM  TauxDeChanges WHERE Date = @DateValue", conn);
                        command.Parameters.Add(new SqlParameter("DateValue", tmpDate));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows != false)
                            {
                                reader.Read();
                                if ((double)reader[0] != -1)
                                    return (double)reader[0];
                            }
                        }
                        maxTry++;
                    }
                }
            }
            return -1;
        }

        internal void setCorrTDC(int id, double corrEstoxx, double corrSP500, double corrN225, double corrHANG, double corrFTSE, double corrASX, double corrEURUSD, double corrEURJPY, double corrEURHKD, double corrEURGBP, double corrEURAUD)
        {
            Indice indAmodifier = bdd.Indices.FirstOrDefault(indice => indice.Id == id);
            if (indAmodifier != null)
            {
                indAmodifier.corrTDCESTOXX = corrEstoxx;
                indAmodifier.corrTDCSP500 = corrSP500;
                indAmodifier.corrTDCN225 = corrN225;
                indAmodifier.corrTDCHANG = corrHANG;
                indAmodifier.corrTDCFTSE = corrFTSE;
                indAmodifier.corrTDCASX = corrASX;

                indAmodifier.corrTDCEURUSD = corrEURUSD;
                indAmodifier.corrTDCEURJPY = corrEURJPY;
                indAmodifier.corrTDCEURHKD = corrEURHKD;
                indAmodifier.corrTDCEURGBP = corrEURGBP;
                indAmodifier.corrTDCEURAUD = corrEURAUD;

                bdd.SaveChanges();
            }
        }

            public double[,] getIndexValues(DateTime dateDebut, DateTime dateFin)
        {
            int count = 1;
            double nbDate = GetBusinessDays(dateDebut, dateFin);
            double[,] res = new double[(int)nbDate, 6]; // Order : ASX ESTOX FTSE HANG N225 SP500
            DateTime tmpDate = dateDebut;
            calibrateDate(ref tmpDate);
            // init first row
            res[0, 0] = getSingleData(tmpDate, "ESTOXX");
            res[0, 1] = getSingleData(tmpDate, "SP500");
            res[0, 2] = getSingleData(tmpDate, "N225");
            res[0, 3] = getSingleData(tmpDate, "HANG");
            res[0, 4] = getSingleData(tmpDate, "FTSE");
            res[0, 5] = getSingleData(tmpDate, "ASX");
            tmpDate = tmpDate.AddDays(1);
            calibrateDate(ref tmpDate);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                conn.Open();
                while (tmpDate < dateFin.AddDays(1))
                {
                    SqlCommand command = new SqlCommand("SELECT ESTOXX,SP500,N225,HANG,FTSE,ASX FROM  IndexesAtDates WHERE Date = @DateValue", conn);
                    command.Parameters.Add(new SqlParameter("DateValue", tmpDate));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows != false && reader.FieldCount == 6)
                        {
                            reader.Read();
                            res[count, 0] = fillValue(res[count - 1, 0], (double)reader[0]); //ASX
                            res[count, 1] = fillValue(res[count - 1, 1], (double)reader[1]); //ESTOXX
                            res[count, 2] = fillValue(res[count - 1, 2], (double)reader[2]); //FTSE
                            res[count, 3] = fillValue(res[count - 1, 3], (double)reader[3]); //HANG
                            res[count, 4] = fillValue(res[count - 1, 4], (double)reader[4]); //N225
                            res[count, 5] = fillValue(res[count - 1, 5], (double)reader[5]); //SP500
                        }
                        else
                        {
                            res[count, 0] = res[count - 1, 0];
                            res[count, 1] = res[count - 1, 1];
                            res[count, 2] = res[count - 1, 2];
                            res[count, 3] = res[count - 1, 3];
                            res[count, 4] = res[count - 1, 4];
                            res[count, 5] = res[count - 1, 5];

                        }
                    }
                    tmpDate = tmpDate.AddDays(1);
                    calibrateDate(ref tmpDate);
                    count++;
                }
            }
            return res;
        }


        public double[,] getChangeValues(DateTime dateDebut, DateTime dateFin)
        {
            int count = 1;
            double nbDate = GetBusinessDays(dateDebut, dateFin);
            double[,] res = new double[(int)nbDate, 5]; // Order : ASX ESTOX FTSE HANG N225 SP500
            DateTime tmpDate = dateDebut;
            calibrateDate(ref tmpDate);
            // init first row
            res[0, 0] = getSingleChange(tmpDate, "EURUSD");
            res[0, 1] = getSingleChange(tmpDate, "EURJPY");
            res[0, 2] = getSingleChange(tmpDate, "EURHKD");
            res[0, 3] = getSingleChange(tmpDate, "EURGBP");
            res[0, 4] = getSingleChange(tmpDate, "EURAUD");
            tmpDate = tmpDate.AddDays(1);
            calibrateDate(ref tmpDate);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                conn.Open();
                while (tmpDate < dateFin.AddDays(1))
                {
                    SqlCommand command = new SqlCommand("SELECT EURUSD,EURJPY,EURHKD,EURGBP,EURAUD FROM TauxDeChanges WHERE Date = @DateValue", conn);
                    command.Parameters.Add(new SqlParameter("DateValue", tmpDate));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows != false && reader.FieldCount == 5)
                        {
                            reader.Read();
                            res[count, 0] = fillValue(res[count - 1, 0], (double)reader[0]); //EURUSD
                            res[count, 1] = fillValue(res[count - 1, 1], (double)reader[1]); //EURAUD
                            res[count, 2] = fillValue(res[count - 1, 2], (double)reader[2]); //EURGBP
                            res[count, 3] = fillValue(res[count - 1, 3], (double)reader[3]); //EURJPY
                            res[count, 4] = fillValue(res[count - 1, 4], (double)reader[4]); //EURHKD
                        }
                        else
                        {
                            res[count, 0] = res[count - 1, 0];
                            res[count, 1] = res[count - 1, 1];
                            res[count, 2] = res[count - 1, 2];
                            res[count, 3] = res[count - 1, 3];
                            res[count, 4] = res[count - 1, 4];
                        }
                    }
                    tmpDate = tmpDate.AddDays(1);
                    calibrateDate(ref tmpDate);
                    count++;
                }
            }
            return res;
        }

        internal void ChangeDate(DateTime currDate)
        {
            bdd.Parametres.FirstOrDefault().CurrDate = currDate;
            bdd.SaveChanges();
        }

        private double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }

        private void calibrateDate(ref DateTime tmpDate)
            {
            if (tmpDate.DayOfWeek == DayOfWeek.Saturday)
                tmpDate = tmpDate.AddDays(+2);
            else if (tmpDate.DayOfWeek == DayOfWeek.Sunday)
                tmpDate = tmpDate.AddDays(+1);
        }

        private double fillValue(double prevValue, double currentValue)
        {
            if (currentValue == -1)
                return prevValue;
            else
                return currentValue;
        }

        internal void setPrice(double price)
        {
            bdd.Parametres.FirstOrDefault().Price = price;
            bdd.SaveChanges();
        }
        internal void setPnl(double pnl)
        {
            bdd.Parametres.FirstOrDefault().Pnl = pnl;
            bdd.SaveChanges();
        }
    }
}