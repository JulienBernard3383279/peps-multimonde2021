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
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true,
                                    HtmlEncode = false,
                                    NullDisplayText = "",
                                    DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FinEst { get; set; }

    }
}