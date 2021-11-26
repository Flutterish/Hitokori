namespace osu.Game.Rulesets.Hitokori
{
    public struct TimeInterval
    {
        public double Interval;
        public double Frequency
        {
            get => 1 / Interval;
            set => Interval = 1 / value;
        }

        public double Buffered;
        public void Update(double elapsed)
            => Buffered += elapsed;
        public bool TickNext()
        {
            if (Buffered >= Interval)
            {
                Buffered -= Interval;
                return true;
            }
            return false;
        }
    }
}
