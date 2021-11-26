namespace osu.Game.Rulesets.Hitokori.Replays
{
    public class AutoButton<T> where T : struct
    {
        public readonly T Action;
        public double PressTime { get; set; }
        public bool IsDown { get; set; }

        public AutoButton(T action)
        {
            Action = action;
        }

        public override string ToString()
            => $"{Action} ({(IsDown ? "Down" : "Up")})";
    }
}
