using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestSendOne2
{

    class Program
    {
        //PATH doit être modifié en local pour ajouter le dossier lib à l'intérieur du dossier contenant les sources binaires de la pnl, version pnl-win64-1.9.2
        [DllImport(@"C:\Users\Julien\Desktop\PEPS-2017-2018\TestInterop2\x64\Debug\UnmanagedCppDll.dll")]
        static extern double SendDouble();

        static void Main(string[] args)
        {
            double d = 5.0;
            double i = SendDouble();
            Console.WriteLine(i);
        }
    }
}
