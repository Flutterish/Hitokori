using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDoubleTime : ModDoubleTime {
		public override LocalisableString Description => "Don't stop, won't stop";
		public override double ScoreMultiplier => 1.12;
	}
}
