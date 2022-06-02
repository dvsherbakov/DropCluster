namespace HexagonalWpf
{
    internal class CentralArea
    {
        public double AvgSpot { get; }
        public CustomFileName Name { get; }

        public CentralArea(double ca, string name)
        {
            AvgSpot = ca;
            Name = new CustomFileName(name);
        }
    }
}
