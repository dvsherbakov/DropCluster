using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrepareImageFrm
{
    class OctoShear
    {
        public Dictionary<int, uint[]> Dict = new Dictionary<int, uint[]>();
        public int CenterBrightest;

        public OctoShear(int size, int centerBrightest)
        {
            CenterBrightest = centerBrightest;
            for (var i = 1; i <= 8; i++)
            {
                Dict.Add(i, new uint[size]);
            }
        }
    }

   
}
