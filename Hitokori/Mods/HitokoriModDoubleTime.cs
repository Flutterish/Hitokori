using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDoubleTime : ModDoubleTime {
		public override double ScoreMultiplier => 1.12;
		public override LocalisableString Description => Localisation.ModStrings.DoubleTimeDescription;
	}
}
