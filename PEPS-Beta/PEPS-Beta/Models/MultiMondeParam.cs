using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEPS_Beta.Models
{
    enum Indices { EuroStoxx50, SP500, NIKKEI225, HANGSENG, FTSE100, SPASX200 };

    public class MultiMondeParam
    {
        #region attributs
        private int nbIndices;

        private readonly DateTime origin; // = new DateTime(01, 10, 2015);

        private DateTime[] constatations; //= new DateTime[6] { new DateTime(07, 10, 2016), new DateTime(13, 10, 2017), new DateTime(19, 10, 2018), new DateTime(25, 10, 2019), new DateTime(30, 10, 2020), new DateTime(05, 11, 2021) };

        private DateTime currDate;

        private int nbSamples;

        private List<Indice> indices;
        #endregion

        #region properties

        protected DateTime Origin => origin;

        protected DateTime[] Constatations { get => constatations; }

        protected DateTime CurrDate { get => currDate; set => currDate = value; }

        public int NbSamples { get => nbSamples; set => nbSamples = value; }

        public int NbIndices { get => nbIndices; set => nbIndices = value; }

        public virtual List<Indice> Indices { get => indices; set => indices = value; }
        #endregion
    }
}