using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods
{
    public class HitokoriModHalfTime : ModHalfTime
    {
        public override string Description => "But not half the fun!";
        public override double ScoreMultiplier => 0.3;
    }
}
