using System;
using System.ComponentModel.DataAnnotations;

namespace PEPS_Beta.ViewModels
{
    public class EstimationViewModel
    {
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true,
                                    HtmlEncode = false,
                                    NullDisplayText = "",
                                    DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DebutEst { get; set; }
        public DateTime FinEst { get => finEst; set => finEst = value; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true,
                                    HtmlEncode = false,
                                    NullDisplayText = "",
                                    DataFormatString = "{0:MM/dd/yyyy}")]
        private DateTime finEst = new DateTime(2015, 10, 21);

    }
}