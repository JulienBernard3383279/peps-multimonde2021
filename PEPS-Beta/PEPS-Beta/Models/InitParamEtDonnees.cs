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
            ds.FillDataHtml("full",2000,2000);
            IndexesAtDate fillIndexDataBase;
            TauxDeChange fillChangeDataBase;
            double[] index = new double[6];
            double[] change = new double[5];
            var allIndexKeys = ds.asx.Keys.Union(ds.estox.Keys).Union(ds.ftse.Keys).Union(ds.hang.Keys).Union(ds.n225.Keys).Union(ds.sp500.Keys);
            foreach( DateTime key in allIndexKeys)
            {
                #region keyCheckIndex
                try
                {
                    index[0] = ds.estox[key];
                } catch (KeyNotFoundException) {
                    index[0] = -1;
                }
                try
                {
                    index[1] = ds.ftse[key];
                }
                catch (KeyNotFoundException)
                {
                    index[1] = -1;
                }
                try
                {
                    index[2] = ds.sp500[key];
                }
                catch (KeyNotFoundException)
                {
                    index[2] = -1;
                }
                try
                {
                    index[3] = ds.n225[key];
                }
                catch (KeyNotFoundException)
                {
                    index[3] = -1;
                }
                try
                {
                    index[4] = ds.hang[key];
                }
                catch (KeyNotFoundException)
                {
                    index[4] = -1;
                }
                try
                {
                    index[5] = ds.asx[key];
                } 
                catch (KeyNotFoundException)
                {
                    index[5] = -1;
                }
                #endregion keyCheck
                fillIndexDataBase = new IndexesAtDate(key, index[5], index[0], index[1], index[2], index[3], index[4]);
               

                context.IndexesValue.Add(fillIndexDataBase);
                //newParam.IndexesValue.Add(fillIndexDataBase);
            }
            var allChangeKeys = ds.EurAud.Keys.Union(ds.EurGbp.Keys).Union(ds.EurHkd.Keys).Union(ds.EurJpy.Keys).Union(ds.EurUsd.Keys);
            foreach(DateTime key in allChangeKeys)
            {
                #region keyCheckChange
                try
                {
                    change[0] = ds.EurUsd[key];
                }
                catch (KeyNotFoundException)
                {
                    change[0] = -1;
                }
                try
                {
                    change[1] = ds.EurAud[key];
                }
                catch (KeyNotFoundException)
                {
                    change[1] = -1;
                }
                try
                {
                    change[2] = ds.EurGbp[key];
                }
                catch (KeyNotFoundException)
                {
                    change[2] = -1;
                }
                try
                {
                    change[3] = ds.EurJpy[key];
                }
                catch (KeyNotFoundException)
                {
                    change[3] = -1;
                }
                try
                {
                    change[4] = ds.EurHkd[key];
                }
                catch (KeyNotFoundException)
                {
                    change[4] = -1;
                }
                #endregion keyCheckChange
                fillChangeDataBase = new TauxDeChange(key, change[0], change[1], change[2], change[3], change[4]);
                context.GetTaux.Add(fillChangeDataBase);

            }
            newParam.NbIndices = 6;
            newParam.NbSamples = 1000;

            PortefeuilleCouverture newPort = new PortefeuilleCouverture();
            context.GetPort.Add(newPort);

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

            base.Seed(context);
            context.SaveChanges();
        }
    }
}