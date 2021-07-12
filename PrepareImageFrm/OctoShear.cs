using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;

namespace PrepareImageFrm
{
    class OctoShear
    {
        private readonly int _size;
        public Dictionary<int, int[]> Dict = new Dictionary<int, int[]>();
        public int CenterBrightest;
        public RotatedRect Shape;

        public OctoShear(int size, int centerBrightest, RotatedRect shape)
        {
            _size = size;
            Shape = shape;
            CenterBrightest = centerBrightest;
            for (var i = 1; i <= 8; i++)
            {
                Dict.Add(i, new int[size]);
            }
        }

        public int[] GetProfile()
        {
            var res = new List<int> { CenterBrightest };

            for (var i = 1; i < _size; i++)
            {
                var lst = Dict.Keys.Select(t => Dict[t][i]).ToList();
                res.Add((int)lst.Average(x => x));
            }

            return res.ToArray();
        }

        public float Diam => (Shape.Size.Height + Shape.Size.Width) / 2;

        public uint AvgBrightest()
        {
            var tmp = new List<int>();
            var pr = GetProfile();
            for (var i=0; i<Diam/2; i++)
                tmp.Add(pr[i]);
            return (uint)tmp.Average(x=>x);
        }


    }


}
