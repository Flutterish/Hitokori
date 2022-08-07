using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNightcore : ModNightcore<HitokoriHitObject> {
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.Mod.Strings.NightcoreDescription );

		public override double ScoreMultiplier => 1.12;
	}
}
