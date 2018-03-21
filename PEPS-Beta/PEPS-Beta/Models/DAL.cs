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

        public void Init()
        {
            if (bdd.Indices.Count()  == 0)
            {
                MultiMondeParam newParam = new MultiMondeParam();
                bdd.Parametres.Add(newParam);
                Indice asx = new Indice("asx");
                bdd.Indices.Add(asx);
                newParam.Indices.Add(asx);
                Indice estox = new Indice("estox");
                bdd.Indices.Add(estox);
                newParam.Indices.Add(estox);
                Indice ftse = new Indice("ftse");
                bdd.Indices.Add(ftse);
                newParam.Indices.Add(ftse);
                Indice hang = new Indice("hang");
                bdd.Indices.Add(hang);
                newParam.Indices.Add(hang);
                Indice n225 = new Indice("n225");
                bdd.Indices.Add(n225);
                newParam.Indices.Add(n225);
                Indice sp500 = new Indice("sp500");
                bdd.Indices.Add(sp500);
                newParam.Indices.Add(sp500);

                newParam.NbIndices = 6;
                newParam.NbSamples = 1000;


                bdd.SaveChanges();
            }
        }
    }
}