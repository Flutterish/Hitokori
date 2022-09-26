using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNightcore : ModNightcore<HitokoriHitObject> {
		public override LocalisableString Description => Localisation.ModStrings.NightcoreDescription;

		public override double ScoreMultiplier => 1.12;
	}
}
