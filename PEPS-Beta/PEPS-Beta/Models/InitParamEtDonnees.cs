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
            Indice asx = new Indice("asx");
            context.Indices.Add(asx);
            newParam.Indices.Add(asx);
            Indice estox = new Indice("estox");
            context.Indices.Add(estox);
            newParam.Indices.Add(estox);
            Indice ftse = new Indice("ftse");
            context.Indices.Add(ftse);
            newParam.Indices.Add(ftse);
            Indice hang = new Indice("hang");
            context.Indices.Add(hang);
            newParam.Indices.Add(hang);
            Indice n225 = new Indice("n225");
            context.Indices.Add(n225);
            newParam.Indices.Add(n225);
            Indice sp500 = new Indice("sp500");
            context.Indices.Add(sp500);
            newParam.Indices.Add(sp500);

            newParam.NbIndices = 6;
            newParam.NbSamples = 1000;

            base.Seed(context);

        }
    }
}