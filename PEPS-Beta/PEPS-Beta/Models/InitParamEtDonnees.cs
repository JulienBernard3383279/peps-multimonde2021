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
            Indice estox = new Indice("estox", "eur");
            context.Indices.Add(estox);
            newParam.Indices.Add(estox);
            Indice sp500 = new Indice("sp500", "usd");
            context.Indices.Add(sp500);
            newParam.Indices.Add(sp500);
            Indice n225 = new Indice("n225", "jpy");
            context.Indices.Add(n225);
            newParam.Indices.Add(n225);
            Indice hang = new Indice("hang", "hkd");
            context.Indices.Add(hang);
            newParam.Indices.Add(hang);
            Indice ftse = new Indice("ftse", "gbp");
            context.Indices.Add(ftse);
            newParam.Indices.Add(ftse);
            Indice asx = new Indice("asx", "aud");
            context.Indices.Add(asx);
            newParam.Indices.Add(asx);

            newParam.NbIndices = 6;
            newParam.NbSamples = 1000;

            PortefeuilleCouverture newPort = new PortefeuilleCouverture();
            context.GetPort.Add(newPort);

            base.Seed(context);

        }
    }
}