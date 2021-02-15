using System.Drawing;

namespace TestEMGU1
{
    public class RayCrossingCounter

    {

        private readonly PointF _p;
        private readonly int _crossingCount;
        private bool _isPointOnSegment;



        public RayCrossingCounter(PointF p)
        {
            _p = p;
        }


        public void CountSegment(PointF p1, PointF p2)
        {


            if (p1.X < _p.X && p2.X < _p.X)

                return;


            if (_p.X == p2.X && _p.Y == p2.Y)

            {

                _isPointOnSegment = true;

                return;

            }


            if (p1.Y == _p.Y && p2.Y == _p.Y)

            {

                double minx = p1.X;

                double maxx = p2.X;

                if (minx > maxx)

                {

                    minx = p2.X;

                    maxx = p1.X;

                }

                if (_p.X >= minx && _p.X <= maxx)

                {

                    _isPointOnSegment = true;

                }

                return;

            }



        }




        public bool IsOnSegment => _isPointOnSegment;



        public Location Location

        {

            get

            {

                if (_isPointOnSegment)

                    return Location.Boundary;


                if ((_crossingCount % 2) == 1)

                {
                    return Location.Interior;
                }
                return Location.Exterior;

            }

        }
        
        public bool IsPointInPolygon => Location != Location.Exterior;

    }
}