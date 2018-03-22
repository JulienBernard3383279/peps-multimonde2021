using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    public class InitParamEtDonnees : DropCreateDatabaseAlways<BddContext>
    {
        protected override void Seed(BddContext context)

        {
            MultiMondeParam newParam = new MultiMondeParam();
            context.Parametres.Add(newParam);

            DataStorage ds = new DataStorage();

            Indice estox = new Indice("estox", "eur", ds.estox);
            context.Indices.Add(estox);
            newParam.Indices.Add(estox);
            Indice sp500 = new Indice("sp500", "usd",ds.sp500);
            context.Indices.Add(sp500);
            newParam.Indices.Add(sp500);
            Indice n225 = new Indice("n225", "jpy",ds.n225);
            context.Indices.Add(n225);
            newParam.Indices.Add(n225);
            Indice hang = new Indice("hang", "hkd",ds.hang);
            context.Indices.Add(hang);
            newParam.Indices.Add(hang);
            Indice ftse = new Indice("ftse", "gbp",ds.ftse);
            context.Indices.Add(ftse);
            newParam.Indices.Add(ftse);
            Indice asx = new Indice("asx", "aud",ds.asx);
            context.Indices.Add(asx);
            newParam.Indices.Add(asx);

            newParam.NbIndices = 6;
            newParam.NbSamples = 1000;

            TauxDeChange EURUSD = new TauxDeChange("eur", "usd", ds.EurUsd);
            context.GetTaux.Add(EURUSD);
            //newParam.Taux.Add(EURUSD);
            TauxDeChange EURGBP = new TauxDeChange("eur", "gbp", ds.EurGbp);
            context.GetTaux.Add(EURUSD);
            TauxDeChange EURHKD = new TauxDeChange("eur", "hkd", ds.EurHkd);
            context.GetTaux.Add(EURUSD);
            TauxDeChange EURJPY = new TauxDeChange("eur", "jpy", ds.EurJpy);
            context.GetTaux.Add(EURUSD);
            TauxDeChange EURAUD = new TauxDeChange("eur", "asx", ds.EurAud);
            context.GetTaux.Add(EURUSD);

            base.Seed(context);

        }
    }
}