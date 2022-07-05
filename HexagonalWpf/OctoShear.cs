using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Emgu.CV.Stitching;
using Emgu.CV.Structure;
using HexagonalWpf.utils;

namespace HexagonalWpf
{
    internal class OctoShear
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

        public int[] GetProfileX()
        {
            var res = new List<int>();
            res.AddRange(Dict[1].ToArray().Reverse().ToList());
            res.AddRange(Dict[5].ToArray().ToList());
            return res.ToArray();
        }

        public int[] GetProfileY()
        {
            var res = new List<int>();
            res.AddRange(Dict[3].ToArray().Reverse().ToList());
            res.AddRange(Dict[7].ToArray().ToList());
            return res.ToArray();
        }

        public float Diam => (Shape.Size.Height + Shape.Size.Width) / 2;

        public uint AvgBrightest()
        {
            var tmp = new List<int>();
            var pr = GetProfile();
            for (var i = 0; i < Diam / 2; i++)
                tmp.Add(pr[i]);
            return (uint)tmp.Average(x => x);
        }

        public int GetAvgCenterSpot()
        {
            var tmp = new List<int>();
            foreach (var axis in Dict)
            {
                tmp.Add(axis.Value[0]);
                tmp.Add(axis.Value[1]);
                tmp.Add(axis.Value[2]);
                tmp.Add(axis.Value[3]);
            }

            return (int)tmp.Average(x => x);
        }

        private int GetAvgEnvirons()
        {
            var tmp = new List<int>();
            foreach (var axis in Dict)
            {
                var lIdx = axis.Value.Length;
                tmp.Add(axis.Value[lIdx - 1]);
                tmp.Add(axis.Value[lIdx - 2]);
                tmp.Add(axis.Value[lIdx - 3]);
                tmp.Add(axis.Value[lIdx - 4]);
            }

            return (int)tmp.Average(x => x);
        }

        private int GetTopLimit => (int)(GetAvgCenterSpot() * 0.75);

        public int GetBottomLimit => (int)(GetAvgEnvirons() * 1.25);

        /*public IEnumerable<ShearPoint> GetSide(int axis)
        {
            var res = new List<ShearPoint>();

            var tmp = GetSubList(axis);

            var i = 0;
            foreach (var itm in Dict[axis])
            {
                if (itm > GetBottomLimit && itm < GetTopLimit)
                {
                    res.Add(new ShearPoint
                    {
                        Brightness = itm,
                        X = i
                    });
                }

                i++;
            }

            ;
            return res;
        }*/

        public IEnumerable<ShearPoint> GetSide(int axis)
        {
            var res = new List<ShearPoint>();
            var subRes = new List<ShearPoint>();
            
            for (var i = 0; i < Dict[axis].Length - 1; i++)
            {
                subRes.Add(new ShearPoint { X = i, Brightness = Dict[axis][i] - Dict[axis][i + 1] });
            }

            var maxPos = subRes.Where(y=>y.Brightness==subRes.Max(x => x.Brightness)).Select(t=>t.X).FirstOrDefault();

            for (var i = maxPos - 3; i < maxPos + 4; i++)
            {
                res.Add(new ShearPoint{X=i, Brightness = Dict[axis][i] });
            }

            return res;
        }

        public double AvgDiam =>
            Dict.Keys.Select(ax => (new SimpleLinearRegression(GetSide(ax), Dict[ax].Skip(Math.Max(0, Dict[ax].Count() - 20)).Average()).Zero)).ToList().Average();
    }
}