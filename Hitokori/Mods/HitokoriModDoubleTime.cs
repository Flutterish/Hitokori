using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDoubleTime : ModDoubleTime {
		public override double ScoreMultiplier => 1.12;
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.Mod.Strings.DoubleTimeDescription );
	}
}
