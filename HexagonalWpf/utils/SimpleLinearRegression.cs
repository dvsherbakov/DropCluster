using System;
using System.Collections.Generic;
using System.Linq;

namespace HexagonalWpf.utils
{
    internal class SimpleLinearRegression
    {
        public double StandardDevX, StandardDevY, Correlation, Slope, Interception;
        private readonly double ground;
        public SimpleLinearRegression(IEnumerable<ShearPoint> points, double ground)
        {
            var shearPoints = points as ShearPoint[] ?? points.ToArray();
            StandardDevX = GetStandardDeviation(shearPoints.Select(x => (double)x.X).ToArray());
            StandardDevY = GetStandardDeviation(shearPoints.Select(y => (double)y.Brightness).ToArray());
            Correlation = GetCorrelation(shearPoints.Select(x => (double)x.X).ToArray(),
                shearPoints.Select(y => (double)y.Brightness).ToArray());
            Slope = Correlation * StandardDevY / StandardDevX;
            Interception = shearPoints.Average(y => y.Brightness) - Slope * shearPoints.Average(x => x.X);
            this.ground = ground;
        }


        public double Zero => (ground - Interception) / Slope;

        private static double GetStandardDeviation(IReadOnlyList<double> arr)
        {
            var mean = arr.Average();
            return Math.Sqrt(arr.Select(x => Math.Pow(x - mean, 2)).Average());
        }

        private static double GetCorrelation(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
        {
            var xMean = x.Average();
            var yMean = y.Average();
            var dX = x.Select(a => a - xMean).ToArray();
            var dY = y.Select(b => b - yMean).ToArray();
            
            var xy = new double[x.Count];
            for (var i = 0; i < x.Count; i++)
            {
                xy[i] = dX[i] * dY[i];
            }

            var xSquare = dX.Select(a => Math.Pow(a, 2));
            var ySquare = dY.Select(b => Math.Pow(b, 2));

            return xy.Sum() / Math.Sqrt(xSquare.Sum() * ySquare.Sum());
        }
    }
}