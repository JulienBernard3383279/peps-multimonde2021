using System;
using System.ComponentModel.DataAnnotations;

namespace PEPS_Beta.ViewModels
{
    public class EstimationViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime DebutEst { get; set; }
        public DateTime FinEst { get => finEst; set => finEst = value; }

        [DataType(DataType.DateTime)]
        private DateTime finEst = new DateTime(2015, 10, 21);

    }
}