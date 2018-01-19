using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace InterfaceWeb.Controllers
{

    public class HomeController : Controller
    {
        [DllImport(@"C:\Users\Julien\Desktop\PEPS-2017-2018\TestInterop2\x64\Debug\UnmanagedCppDll.dll")]
        static extern double SendDouble(double d);

        // GET: Home
        public ActionResult Index()
        {
            double d = 7.0;
            ViewData["d"] = d;
            double i = SendDouble(d);
            ViewData["result"] = i;
            return View();
        }
    }
}