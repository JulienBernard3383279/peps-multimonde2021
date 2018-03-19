using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPS_Beta.Models
{
    interface IDAL : IDisposable
    {
        List<Indice> GetIndices();
        MultiMondeParam GetParams();

        void Init();
    }
}
