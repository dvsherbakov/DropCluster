using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermoCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            var f = new PrepareCsv();
            f.OpenFile(@"E:\+Data\тепловизор\1_1.csv");
        }
    }
}
