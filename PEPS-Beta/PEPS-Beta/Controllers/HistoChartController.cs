using System.Web.Helpers;
using PEPS_Beta.Models;
using PEPS_Beta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using PricerDll.CustomTests;

namespace PEPS_Beta.Controllers
{
    public class HistoChartController : Controller
    {
        // GET: User
        public ActionResult Index(int? page)
        {
            var model = new HistoricalChartViewModel
            {
                Chart = GetChart()
            };
            return View(model);
        }
        private Chart GetChart()
        {
            DAL dal = new DAL();
            DateTime date1 = DateTime.Parse("10/10/2010");
            DateTime date2 = DateTime.Parse("10/10/2012");
            double[,] test = dal.getIndexValues(date1, date2);
            
            return new Chart(600, 400, ChartTheme.Blue)
                .AddTitle("Historical Data")
                .AddLegend()
                .AddSeries(
                    name: "WebSite",
                    chartType: "Pie",
                    xValue: new[] { "Digg", "DZone", "DotNetKicks", "StumbleUpon" },
                    yValues: new[] { "150000", "180000", "120000", "250000" });
        }
    }
}