using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNightcore : ModNightcore<HitokoriHitObject> {
		public override string Description => base.Description;
		public override double ScoreMultiplier => 1.12;
	}
}
