using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNightcore : ModNightcore<HitokoriHitObject> {
		public override LocalisableString Description => base.Description;
		public override double ScoreMultiplier => 1.12;
	}
}
