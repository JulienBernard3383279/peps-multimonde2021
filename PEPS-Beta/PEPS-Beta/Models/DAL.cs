using System;
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
                Indice estox = new Indice("estox", "eur");
                bdd.Indices.Add(estox);
                newParam.Indices.Add(estox);
                Indice sp500 = new Indice("sp500", "usd");
                bdd.Indices.Add(sp500);
                newParam.Indices.Add(sp500);
                Indice n225 = new Indice("n225", "jpy");
                bdd.Indices.Add(n225);
                newParam.Indices.Add(n225);
                Indice hang = new Indice("hang", "hkd");
                bdd.Indices.Add(hang);
                newParam.Indices.Add(hang);
                Indice ftse = new Indice("ftse", "gbp");
                bdd.Indices.Add(ftse);
                newParam.Indices.Add(ftse);
                Indice asx = new Indice("asx", "aud");
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
            String hello = date.ToString();
            int maxTry = 0;
            if (indexName.Equals("ASX") || indexName.Equals("ESTOX") || indexName.Equals("SP500") || indexName.Equals("N225") || indexName.Equals("HANG") || indexName.Equals("FTSE"))
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
                            double PINGVALUE = (double)reader[0];
                            if ((double)reader[0] != -1)
                                return (double)reader[0];
                        }
                    }
                    while (maxTry < 10)
                    {
                        tmpDate = date.AddDays(-1);
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
    }
}