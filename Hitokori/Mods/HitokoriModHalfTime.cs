using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHalfTime : ModHalfTime {
		public override LocalisableString Description => "But not half the fun!";
		public override double ScoreMultiplier => 0.3;
	}
}
