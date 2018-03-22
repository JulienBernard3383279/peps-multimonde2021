using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            if (bdd.Indices.Count()  == 0)
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
            if (indAmodifier!=null)
            {
                indAmodifier.InterestRateThisArea = interestRateThisArea;
                indAmodifier.Vol = vol;
                bdd.SaveChanges();
            }
        }
    }
}