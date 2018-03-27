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
        public int Id { get; set; }

        private int nbIndices;

        private DateTime origin = new DateTime(2015, 10, 01);

        private DateTime[] constatations = new DateTime[6] { new DateTime(2016, 10, 07), new DateTime(2017, 10, 13), new DateTime(2018, 10, 19), new DateTime(2019, 10, 25), new DateTime(2020, 10, 30), new DateTime(2021, 11, 05) };

        private DateTime currDate = new DateTime(2015, 10, 01);

        private int nbSamples;

        private List<Indice> indices;
        #endregion

        #region properties


        public DateTime[] Constatations { get => constatations; }


        public int NbSamples { get => nbSamples; set => nbSamples = value; }

        public int NbIndices { get => nbIndices; set => nbIndices = value; }

        public virtual List<Indice> Indices { get => indices; set => indices = value; }
        #endregion

        private DateTime endDate = new DateTime(2021, 11, 05);


        public DateTime CurrDate { get => currDate; set => currDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public DateTime Origin { get => origin; set => origin = value; }

        public MultiMondeParam()
        {
            this.constatations = new DateTime[6];
            this.indices = new List<Indice>();
        }
    }
}