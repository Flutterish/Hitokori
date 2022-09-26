using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHalfTime : ModHalfTime {
		public override double ScoreMultiplier => 0.4;
		public override LocalisableString Description => Localisation.ModStrings.HalfTimeDescription;
	}
}
