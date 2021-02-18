using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Converter
{
    interface ICSVConvert
    {
        void Convert<T>(List<T> items, string fileName);
    }
}
