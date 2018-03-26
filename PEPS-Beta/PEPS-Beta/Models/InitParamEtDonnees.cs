﻿using System;
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
            ds.FillDataHtml("full",2500,2500);
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
                    if (ds.estox[key] != 0)
                        index[0] = ds.estox[key];
                    else
                        index[0] = -1;
                }
                catch (KeyNotFoundException) {
                    index[0] = -1;
                }
                try
                {
                    if (ds.ftse[key] != 0)
                        index[1] = ds.ftse[key];
                    else
                        index[1] = -1;
                }
                catch (KeyNotFoundException)
                {
                    index[1] = -1;
                }
                try
                {
                    if (ds.sp500[key] != 0)
                        index[2] = ds.sp500[key];
                    else
                        index[2] = -1;
                }
                catch (KeyNotFoundException)
                {
                    index[2] = -1;
                }
                try
                {
                    if (ds.n225[key] != 0)
                        index[3] = ds.n225[key];
                    else
                        index[3] = -1;
                }
                catch (KeyNotFoundException)
                {
                    index[3] = -1;
                }
                try
                {
                    if (ds.hang[key] != 0)
                        index[4] = ds.hang[key];
                    else
                        index[4] = -1;
                }
                catch (KeyNotFoundException)
                {
                    index[4] = -1;
                }
                try
                {
                    if (ds.asx[key] != 0)
                        index[5] = ds.asx[key];
                    else
                        index[5] = -1;
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
                    if(ds.EurUsd[key] != 0)
                        change[0] = ds.EurUsd[key];
                    else
                        change[0] = -1;
                }
                catch (KeyNotFoundException)
                {
                    change[0] = -1;
                }
                try
                {
                    if (ds.EurAud[key] != 0)
                        change[1] = ds.EurAud[key];
                    else
                        change[1] = -1;
                }
                catch (KeyNotFoundException)
                {
                    change[1] = -1;
                }
                try
                {
                    if (ds.EurGbp[key] != 0)
                        change[2] = ds.EurGbp[key];
                    else
                        change[2] = -1;
                }
                catch (KeyNotFoundException)
                {
                    change[2] = -1;
                }
                try
                {
                    if (ds.EurJpy[key] != 0)
                        change[3] = ds.EurJpy[key];
                    else
                        change[3] = -1;
                }
                catch (KeyNotFoundException)
                {
                    change[3] = -1;
                }
                try
                {
                    if (ds.EurHkd[key] != 0)
                        change[4] = ds.EurHkd[key];
                    else
                        change[4] = -1;
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
            base.Seed(context);
            context.SaveChanges();
        }
    }
}